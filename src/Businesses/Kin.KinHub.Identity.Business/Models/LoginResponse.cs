namespace Kin.KinHub.Identity.Business.Models;

public sealed class LoginResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required int ExpiresIn { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
}
