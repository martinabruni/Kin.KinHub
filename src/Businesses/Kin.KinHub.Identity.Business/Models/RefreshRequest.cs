namespace Kin.KinHub.Identity.Business.Models;

public sealed class RefreshRequest
{
    public required string RefreshToken { get; init; }
}
