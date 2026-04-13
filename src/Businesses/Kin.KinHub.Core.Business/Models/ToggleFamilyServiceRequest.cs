namespace Kin.KinHub.Core.Business;

public sealed class ToggleFamilyServiceRequest
{
    public required int ServiceId { get; init; }
    public required bool IsActive { get; init; }
}
