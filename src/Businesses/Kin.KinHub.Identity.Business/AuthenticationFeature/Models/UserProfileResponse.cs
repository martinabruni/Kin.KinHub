namespace Kin.KinHub.Identity.Business.AuthenticationFeature;

public sealed class UserProfileResponse
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
}
