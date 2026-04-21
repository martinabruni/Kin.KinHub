using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain.FamilyFeature;

public sealed class FamilyRole : BaseActivableEntity<int>
{
    public required string Name { get; set; }
}
