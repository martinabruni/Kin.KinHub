namespace Kin.KinHub.Core.Domain.AI;

public sealed record RecipeSuggestion
{
    public required RecipeAssistantRecipe Recipe { get; init; }
    public required int MatchPercentage { get; init; }
    public required IReadOnlyList<RecipeAssistantIngredient> MissingIngredients { get; init; }
}
