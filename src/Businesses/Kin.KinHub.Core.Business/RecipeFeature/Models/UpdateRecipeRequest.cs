namespace Kin.KinHub.Core.Business.RecipeFeature;

public sealed class UpdateRecipeRequest
{
    public required string Name { get; init; }
    public string? Backstory { get; init; }
    public required TimeSpan FinalTime { get; init; }
    public required int Portions { get; init; }
}
