using Azure;
using Azure.AI.OpenAI;
using Kin.KinHub.Core.Domain.AI;
using Kin.KinHub.Core.Domain.Interfaces.AI;
using Kin.KinHub.OpenAi.Common;
using OpenAI.Chat;
using System.Globalization;
using System.Text.Json;

namespace Kin.KinHub.OpenAi.Services;

internal sealed class OpenAiRecipeAssistantService : IRecipeAssistantService
{
    private const string SystemPrompt = """
        You are a recipe assistant. You process recipe-related tasks and respond exclusively with a single valid JSON object. No markdown, no code blocks, no prose outside of JSON.

        GLOBAL RULES:
        - Always include "task_type" in the response, matching the value from the input.
        - Never invent implausible or fictional ingredients.
        - Preserve the original unit of measure when provided; default to metric otherwise.
        - Express "final_time" as a duration string in HH:MM:SS format (e.g., "00:30:00", "01:15:00").
        - Ingredient shape: { "name": string, "quantity": number, "unit": string }
        - Step shape: { "order": integer (starting at 1), "description": string }
        - If a value cannot be determined: use null for nullable strings, 0 for unknown quantities, "unknown" for missing units.
        - Never return partial JSON. If processing fails, return a valid error response as specified below.

        ---

        TASK: recipe_suggestion

        Input:
        {
          "task_type": "recipe_suggestion",
          "fridge_ingredients": [ { "name": string, "quantity": number, "unit": string } ],
          "family_recipes": [ { "name": string, "backstory": string | null, "final_time": string, "portions": number, "ingredients": [...], "steps": [...] } ]  // optional
        }

        Output:
        {
          "task_type": "recipe_suggestion",
          "suggestions": [
            {
              "recipe": {
                "name": string,
                "backstory": string | null,
                "final_time": string,
                "portions": number,
                "ingredients": [ { "name": string, "quantity": number, "unit": string } ],
                "steps": [ { "order": number, "description": string } ]
              },
              "match_percentage": integer,
              "missing_ingredients": [ { "name": string, "quantity": number, "unit": string } ]
            }
          ]
        }

        Rules:
        - Return at most 3 suggestions, sorted by match_percentage descending.
        - match_percentage: integer 0-100 representing the fraction of required ingredients available in the fridge (by name or close similarity).
        - missing_ingredients: ingredients required by the recipe that are absent or insufficient in the fridge.
        - When family_recipes is provided and non-empty: score each family recipe against fridge_ingredients and include the best-matching ones first. Fill remaining slots (up to 3 total) with generic well-known recipes only if fewer than 3 family recipes score above 0%.
        - When family_recipes is absent or empty: suggest up to 3 real, well-known, or plausible recipes based solely on fridge_ingredients.
        - Do not hallucinate recipes or ingredients. Only suggest real, plausible recipes.
        - If fridge_ingredients is empty or no plausible recipe can be assembled, return "suggestions": [].

        ---

        TASK: recipe_parsing

        Input:
        {
          "task_type": "recipe_parsing",
          "raw_text": string
        }

        Output (success):
        {
          "task_type": "recipe_parsing",
          "recipe": {
            "name": string,
            "backstory": string | null,
            "final_time": string,
            "portions": number,
            "ingredients": [ { "name": string, "quantity": number, "unit": string } ],
            "steps": [ { "order": number, "description": string } ]
          },
          "error": null
        }

        Output (failure):
        {
          "task_type": "recipe_parsing",
          "recipe": null,
          "error": "unable_to_parse"
        }

        Rules:
        - Parse from free text, a pasted recipe, or recipe-like content (including scraped URL descriptions).
        - If a quantity is not mentioned for an ingredient, use 0 for quantity and "unknown" for unit.
        - Convert bullet points, paragraphs, or numbered lists into ordered steps starting at order 1.
        - If the input is gibberish, too short, or clearly not a recipe, return recipe: null with error: "unable_to_parse".

        ---

        TASK: recipe_adaptation

        Input:
        {
          "task_type": "recipe_adaptation",
          "recipe": { ... },
          "constraints": [ string ]
        }

        Output:
        {
          "task_type": "recipe_adaptation",
          "original_recipe": { ... },
          "adapted_recipe": { ... },
          "changes": [
            { "type": "substitution" | "removal" | "addition" | "scaling", "description": string }
          ]
        }

        Rules:
        - original_recipe must be an exact, unmodified copy of the input recipe.
        - Apply every constraint coherently. Examples:
            "no eggs" -> substitute with a plausible alternative (e.g. flaxseed egg, aquafaba).
            "vegan" -> replace all animal-derived ingredients with plant-based alternatives.
            "serve N people" -> scale all ingredient quantities proportionally and update portions to N.
        - Do not remove a structural ingredient without a suitable substitution.
        - Recalculate portions and scale ingredient quantities when a serving-size constraint is applied.
        - List every modification in changes, one entry per distinct change.
        - Preserve final_time unless a constraint explicitly changes cooking time.
        - If a constraint is irreconcilable (e.g. "no flour" for a bread recipe), apply the closest possible adaptation and document it in changes.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    private readonly ChatClient _chatClient;

    public OpenAiRecipeAssistantService(OpenAiOptions options)
    {
        var client = new AzureOpenAIClient(new Uri(options.Endpoint), new AzureKeyCredential(options.ApiKey));
        _chatClient = client.GetChatClient(options.ChatDeploymentName);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<RecipeSuggestion>> SuggestRecipesAsync(
        IReadOnlyList<RecipeAssistantIngredient> fridgeIngredients,
        IReadOnlyList<RecipeAssistantRecipe>? familyRecipes = null,
        CancellationToken cancellationToken = default)
    {
        var input = new
        {
            task_type = "recipe_suggestion",
            fridge_ingredients = fridgeIngredients.Select(i => new { i.Name, i.Quantity, i.Unit }),
            family_recipes = familyRecipes?.Select(MapToJsonObject),
        };

        var json = await SendAsync(JsonSerializer.Serialize(input, JsonOptions), temperature: 0.7f, cancellationToken);
        var response = JsonSerializer.Deserialize<SuggestionResponse>(json, JsonOptions)!;

        return response.Suggestions
            .Select(MapToSuggestion)
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<RecipeAssistantRecipe?> ParseRecipeAsync(
        string rawText,
        CancellationToken cancellationToken = default)
    {
        var input = new { task_type = "recipe_parsing", raw_text = rawText };

        var json = await SendAsync(JsonSerializer.Serialize(input, JsonOptions), temperature: 0.3f, cancellationToken);
        var response = JsonSerializer.Deserialize<ParseResponse>(json, JsonOptions)!;

        return response.Recipe is null ? null : MapToRecipe(response.Recipe);
    }

    /// <inheritdoc/>
    public async Task<RecipeAdaptationResult> AdaptRecipeAsync(
        RecipeAssistantRecipe recipe,
        IReadOnlyList<string> constraints,
        CancellationToken cancellationToken = default)
    {
        var input = new
        {
            task_type = "recipe_adaptation",
            recipe = MapToJsonObject(recipe),
            constraints,
        };

        var json = await SendAsync(JsonSerializer.Serialize(input, JsonOptions), temperature: 0.3f, cancellationToken);
        var response = JsonSerializer.Deserialize<AdaptationResponse>(json, JsonOptions)!;

        return new RecipeAdaptationResult
        {
            OriginalRecipe = MapToRecipe(response.OriginalRecipe),
            AdaptedRecipe = MapToRecipe(response.AdaptedRecipe),
            Changes = response.Changes
                .Select(c => new RecipeChange { Type = c.Type, Description = c.Description })
                .ToList(),
        };
    }

    private async Task<string> SendAsync(string userMessage, float temperature, CancellationToken cancellationToken)
    {
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
            Temperature = temperature,
        };

        var result = await _chatClient.CompleteChatAsync(
            [
                new SystemChatMessage(SystemPrompt),
                new UserChatMessage(userMessage),
            ],
            options,
            cancellationToken);

        return result.Value.Content[0].Text;
    }

    private static object MapToJsonObject(RecipeAssistantRecipe recipe) =>
        new
        {
            recipe.Name,
            recipe.Backstory,
            recipe.FinalTime,
            recipe.Portions,
            Ingredients = recipe.Ingredients.Select(i => new { i.Name, i.Quantity, i.Unit }),
            Steps = recipe.Steps.Select(s => new { s.Order, s.Description }),
        };

    private static RecipeAssistantIngredient MapToIngredient(IngredientJson j) =>
        new() { Name = j.Name, Quantity = j.Quantity, Unit = j.Unit };

    private static RecipeAssistantStep MapToStep(StepJson j) =>
        new() { Order = j.Order, Description = j.Description };

    private static RecipeAssistantRecipe MapToRecipe(RecipeJson j) =>
        new()
        {
            Name = j.Name,
            Backstory = j.Backstory,
            FinalTime = TimeSpan.TryParse(j.FinalTime, CultureInfo.InvariantCulture, out var ts) ? ts : TimeSpan.Zero,
            Portions = j.Portions,
            Ingredients = j.Ingredients.Select(MapToIngredient).ToList(),
            Steps = j.Steps.Select(MapToStep).ToList(),
        };

    private static RecipeSuggestion MapToSuggestion(SuggestionItem s) =>
        new()
        {
            Recipe = MapToRecipe(s.Recipe),
            MatchPercentage = s.MatchPercentage,
            MissingIngredients = s.MissingIngredients.Select(MapToIngredient).ToList(),
        };

    private sealed record IngredientJson(string Name, decimal Quantity, string Unit);
    private sealed record StepJson(int Order, string Description);
    private sealed record RecipeJson(
        string Name,
        string? Backstory,
        string FinalTime,
        int Portions,
        IReadOnlyList<IngredientJson> Ingredients,
        IReadOnlyList<StepJson> Steps);
    private sealed record SuggestionItem(RecipeJson Recipe, int MatchPercentage, IReadOnlyList<IngredientJson> MissingIngredients);
    private sealed record SuggestionResponse(string TaskType, IReadOnlyList<SuggestionItem> Suggestions);
    private sealed record ParseResponse(string TaskType, RecipeJson? Recipe, string? Error);
    private sealed record ChangeJson(string Type, string Description);
    private sealed record AdaptationResponse(
        string TaskType,
        RecipeJson OriginalRecipe,
        RecipeJson AdaptedRecipe,
        IReadOnlyList<ChangeJson> Changes);
}
