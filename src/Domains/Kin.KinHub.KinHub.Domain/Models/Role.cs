using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Domain;

public sealed class Role : BaseActivableEntity<int>
{
    public string? Name { get; set; }
}
