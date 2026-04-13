namespace Kin.KinHub.Identity.Domain.Models;

public sealed class IdentityProvider : BaseActivableEntity<int>
{
    public string? Name { get; set; }
    public string? Label { get; set; }
}
