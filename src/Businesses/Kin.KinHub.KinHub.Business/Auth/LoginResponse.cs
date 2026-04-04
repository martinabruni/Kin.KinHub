namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class LoginResponse
{
    public required Guid SessionId { get; init; }
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string? DisplayName { get; init; }
}
