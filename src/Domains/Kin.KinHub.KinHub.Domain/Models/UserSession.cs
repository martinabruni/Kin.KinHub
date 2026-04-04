using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Domain.Models;

public sealed class UserSession : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required int ProviderId { get; set; }
}
