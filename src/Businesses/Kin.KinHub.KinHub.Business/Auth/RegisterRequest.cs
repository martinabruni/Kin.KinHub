namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; }
}
