namespace Kin.KinHub.Identity.Business.Models;

public sealed class UserProfileResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
