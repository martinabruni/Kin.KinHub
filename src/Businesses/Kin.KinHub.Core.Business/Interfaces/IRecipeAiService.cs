using Kin.KinHub.Core.Business.Common;
using Kin.KinHub.Core.Domain.AI;

namespace Kin.KinHub.Core.Business;

/// <summary>
/// Orchestrates AI-powered recipe features: suggestion from fridge contents, parsing from raw text, and constraint-based adaptation.
/// </summary>
public interface IRecipeAiService
{
    /// <summary>Suggests up to 3 recipes based on the fridge contents, prioritizing the family's saved recipes.</summary>
    Task<Result<IReadOnlyList<RecipeSuggestion>>> SuggestRecipesAsync(
        Guid fridgeId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>Parses a recipe from free text or unstructured input. Returns null in the result value if the text cannot be parsed.</summary>
    Task<Result<RecipeAssistantRecipe?>> ParseRecipeAsync(
        string rawText,
        CancellationToken cancellationToken = default);

    /// <summary>Adapts a saved recipe according to the given constraints (e.g. "vegan", "no eggs", "serve 5").</summary>
    Task<Result<RecipeAdaptationResult>> AdaptRecipeAsync(
        Guid recipeId,
        IReadOnlyList<string> constraints,
        Guid userId,
        CancellationToken cancellationToken = default);
}
