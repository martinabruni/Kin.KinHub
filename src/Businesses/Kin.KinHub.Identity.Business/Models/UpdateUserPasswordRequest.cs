namespace Kin.KinHub.Identity.Business.Models;

public sealed class UpdateUserPasswordRequest
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}
