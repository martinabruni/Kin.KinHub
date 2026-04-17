namespace Kin.KinHub.Core.Business.FamilyFeature;

public sealed class ToggleFamilyServiceRequest
{
    public required int ServiceId { get; init; }
    public required bool IsActive { get; init; }
}
