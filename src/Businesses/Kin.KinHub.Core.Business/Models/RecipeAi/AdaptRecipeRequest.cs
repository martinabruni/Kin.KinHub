namespace Kin.KinHub.Core.Business;

public sealed class AdaptRecipeRequest
{
    public required Guid RecipeId { get; init; }
    public required IReadOnlyList<string> Constraints { get; init; }
}
