namespace Kin.KinHub.Core.Business;

public sealed class UpdateFamilyResponse
{
    public required Guid FamilyId { get; init; }
    public required string Name { get; init; }
}
