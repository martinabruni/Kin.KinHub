using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Domain.Models;

public class IdentityUserCredential : BaseDeletableEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string? PasswordHash { get; set; }
}
