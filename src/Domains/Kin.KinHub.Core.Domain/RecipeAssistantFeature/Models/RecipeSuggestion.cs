namespace Kin.KinHub.Core.Domain.RecipeAssistantFeature;

public sealed record RecipeSuggestion
{
    public required RecipeAssistantRecipe Recipe { get; init; }
    public required int MatchPercentage { get; init; }
    public required IReadOnlyList<RecipeAssistantIngredient> MissingIngredients { get; init; }
}
