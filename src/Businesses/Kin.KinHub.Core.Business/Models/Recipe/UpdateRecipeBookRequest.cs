namespace Kin.KinHub.Core.Business;

public sealed class UpdateRecipeBookRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
