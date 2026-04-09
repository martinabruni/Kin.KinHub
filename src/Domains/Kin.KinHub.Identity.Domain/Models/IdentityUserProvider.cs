namespace Kin.KinHub.Identity.Domain.Models;

public sealed class IdentityUserProvider : BaseDeletableEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required int ProviderId { get; set; }
    public required string ProviderUserId { get; set; }
}
