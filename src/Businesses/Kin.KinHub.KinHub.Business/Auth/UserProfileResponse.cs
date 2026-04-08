namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class UserProfileResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
