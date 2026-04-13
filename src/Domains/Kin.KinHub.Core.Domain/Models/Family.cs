using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain;

public sealed class Family : BaseDeletableEntity<Guid>
{
    public required string Name { get; set; }
    public required Guid UserId { get; set; }
    public required string AdminCodeHash { get; set; }
}
