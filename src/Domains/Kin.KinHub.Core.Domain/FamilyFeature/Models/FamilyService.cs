using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain.FamilyFeature;

public sealed class FamilyService : BaseActivableEntity<Guid>
{
    public required Guid FamilyId { get; set; }
    public required int ServiceId { get; set; }
}
