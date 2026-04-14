namespace Kin.KinHub.Identity.Domain.Models;

public sealed class Provider : BaseActivableEntity<int>
{
    public string? Name { get; set; }
    public string? Label { get; set; }
}
