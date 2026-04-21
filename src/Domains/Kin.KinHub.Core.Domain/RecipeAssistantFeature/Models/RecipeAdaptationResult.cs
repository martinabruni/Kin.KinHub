namespace Kin.KinHub.Core.Domain.RecipeAssistantFeature;

public sealed record RecipeAdaptationResult
{
    public required RecipeAssistantRecipe OriginalRecipe { get; init; }
    public required RecipeAssistantRecipe AdaptedRecipe { get; init; }
    public required IReadOnlyList<RecipeChange> Changes { get; init; }
}
