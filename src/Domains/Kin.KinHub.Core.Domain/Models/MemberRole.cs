using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain;

public sealed class MemberRole : BaseActivableEntity<Guid>
{
    public required Guid MemberId { get; set; }
    public required int RoleId { get; set; }
}
