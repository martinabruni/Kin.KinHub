namespace Kin.KinHub.Identity.Domain.Models;

public sealed class IdentityUserCredential : BaseDeletableEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string? PasswordHash { get; set; }
}
