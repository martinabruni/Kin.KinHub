using Kin.KinHub.Core.Domain.AI;

namespace Kin.KinHub.Core.Domain.Interfaces.AI;

/// <summary>
/// Service for AI-powered recipe assistance: suggestion, parsing, and adaptation.
/// </summary>
public interface IRecipeAssistantService
{
    /// <summary>Suggests up to 3 recipes based on the available fridge ingredients, prioritizing the family's saved recipes when provided.</summary>
    Task<IReadOnlyList<RecipeSuggestion>> SuggestRecipesAsync(
        IReadOnlyList<RecipeAssistantIngredient> fridgeIngredients,
        IReadOnlyList<RecipeAssistantRecipe>? familyRecipes = null,
        CancellationToken cancellationToken = default);

    /// <summary>Parses a recipe from free text or unstructured input. Returns null if the text cannot be parsed.</summary>
    Task<RecipeAssistantRecipe?> ParseRecipeAsync(
        string rawText,
        CancellationToken cancellationToken = default);

    /// <summary>Adapts a recipe according to the given constraints (e.g. "vegan", "no eggs", "serve 5").</summary>
    Task<RecipeAdaptationResult> AdaptRecipeAsync(
        RecipeAssistantRecipe recipe,
        IReadOnlyList<string> constraints,
        CancellationToken cancellationToken = default);
}
