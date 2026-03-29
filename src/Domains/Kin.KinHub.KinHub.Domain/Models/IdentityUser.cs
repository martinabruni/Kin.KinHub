using System.ComponentModel.DataAnnotations;
using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Domain.Models;

public sealed class IdentityUser : BaseDeletableEntity<Guid>
{
    public required EmailAddressAttribute Email { get; set; }
    public string? DisplayName { get; set; }
    public bool IsEmailVerified { get; set; }
}
