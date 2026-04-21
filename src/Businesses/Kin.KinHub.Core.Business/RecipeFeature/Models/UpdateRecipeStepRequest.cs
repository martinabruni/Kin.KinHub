namespace Kin.KinHub.Core.Business.RecipeFeature;

public sealed class UpdateRecipeStepRequest
{
    public required int Order { get; init; }
    public required string Description { get; init; }
}
