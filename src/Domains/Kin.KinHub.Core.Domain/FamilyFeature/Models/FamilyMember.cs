using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain.FamilyFeature;

public sealed class FamilyMember : BaseDeletableEntity<Guid>
{
    public required string Name { get; set; }
    public required Guid FamilyId { get; set; }
}
