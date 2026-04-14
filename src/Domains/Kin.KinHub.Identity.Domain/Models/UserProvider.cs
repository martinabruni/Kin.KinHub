namespace Kin.KinHub.Identity.Domain.Models;

public sealed class UserProvider : BaseDeletableEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required int ProviderId { get; set; }
    public required string ProviderUserId { get; set; }
}
