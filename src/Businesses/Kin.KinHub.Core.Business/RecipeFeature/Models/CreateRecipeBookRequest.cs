namespace Kin.KinHub.Core.Business.RecipeFeature;

public sealed class CreateRecipeBookRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
