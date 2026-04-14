namespace Kin.KinHub.Core.Business;

public sealed class RecipeBookResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Guid FamilyId { get; init; }
}
