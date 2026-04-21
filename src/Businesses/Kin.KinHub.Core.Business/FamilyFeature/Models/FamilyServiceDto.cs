namespace Kin.KinHub.Core.Business.FamilyFeature;

public sealed class FamilyServiceDto
{
    public required Guid Id { get; init; }
    public required int ServiceId { get; init; }
    public required string ServiceName { get; init; }
    public required bool IsActive { get; init; }
}
