namespace Kin.KinHub.Core.Business.FamilyFeature;

public sealed class UpdateFamilyResponse
{
    public required Guid FamilyId { get; init; }
    public required string Name { get; init; }
}
