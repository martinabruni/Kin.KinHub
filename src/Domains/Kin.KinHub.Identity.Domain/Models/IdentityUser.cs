using Kin.KinHub.Identity.Domain.Enums;

namespace Kin.KinHub.Identity.Domain.Models;

public sealed class IdentityUser : BaseDeletableEntity<Guid>
{
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public bool IsEmailVerified { get; set; }
    public List<string> Roles { get; set; } = [];
    public UserStatus Status { get; set; } = UserStatus.Active;
}
