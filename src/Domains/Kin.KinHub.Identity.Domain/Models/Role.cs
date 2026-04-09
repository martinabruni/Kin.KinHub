using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain;

public sealed class Role : BaseActivableEntity<int>
{
    public string? Name { get; set; }
}
