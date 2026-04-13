namespace Kin.KinHub.Core.Business;

public sealed class UpdateAdminCodeRequest
{
    public required string CurrentCode { get; init; }
    public required string NewCode { get; init; }
}
