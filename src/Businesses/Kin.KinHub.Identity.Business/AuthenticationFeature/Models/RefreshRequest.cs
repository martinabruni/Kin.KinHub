namespace Kin.KinHub.Identity.Business.AuthenticationFeature;

public sealed class RefreshRequest
{
    public required string RefreshToken { get; init; }
}
