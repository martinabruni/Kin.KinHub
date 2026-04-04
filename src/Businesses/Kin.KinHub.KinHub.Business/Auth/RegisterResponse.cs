namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class RegisterResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
}
