namespace Kin.KinHub.Identity.Business.AuthenticationFeature;

public sealed class RegisterResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}
