namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class LoginResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required int ExpiresIn { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
}
