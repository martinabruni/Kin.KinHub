using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Domain.Models;

public sealed class IdentityProvider : BaseActivableEntity<int>
{
    public string? Name { get; set; }
    public string? Label { get; set; }
}
