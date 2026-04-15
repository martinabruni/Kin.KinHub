namespace Kin.KinHub.Core.Domain.AI;

public sealed record RecipeChange
{
    public required string Type { get; init; }
    public required string Description { get; init; }
}
