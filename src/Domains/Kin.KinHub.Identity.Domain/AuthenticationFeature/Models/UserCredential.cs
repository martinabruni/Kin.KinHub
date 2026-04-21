using Kin.KinHub.Identity.Domain.Common;
namespace Kin.KinHub.Identity.Domain.AuthenticationFeature;

public sealed class UserCredential : BaseDeletableEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string? PasswordHash { get; set; }
}
