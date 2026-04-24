using Kin.KinHub.Core.Domain.RecipeFeature;

namespace Kin.KinHub.Core.Domain.RecipeAssistantFeature;

public sealed record RecipeSuggestion
{
    public required Recipe Recipe { get; init; }
    public required int MatchPercentage { get; init; }
    public required IReadOnlyList<RecipeIngredient> MissingIngredients { get; init; }
}
