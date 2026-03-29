using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Domain.Models;

public sealed class IdentityUserProvider : BaseDeletableEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required int ProviderId { get; set; }
    public required string ProviderUserId { get; set; }
}
