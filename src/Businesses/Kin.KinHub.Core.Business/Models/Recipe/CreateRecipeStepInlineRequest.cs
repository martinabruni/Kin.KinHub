namespace Kin.KinHub.Core.Business;

public sealed class CreateRecipeStepInlineRequest
{
    public required int Order { get; init; }
    public required string Description { get; init; }
}
