namespace Kin.KinHub.Core.Business;

public sealed class FridgeResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid FamilyId { get; init; }
}
