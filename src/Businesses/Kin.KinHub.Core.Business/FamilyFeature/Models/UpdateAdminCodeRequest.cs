namespace Kin.KinHub.Core.Business.FamilyFeature;

public sealed class UpdateAdminCodeRequest
{
    public required string CurrentCode { get; init; }
    public required string NewCode { get; init; }
}
