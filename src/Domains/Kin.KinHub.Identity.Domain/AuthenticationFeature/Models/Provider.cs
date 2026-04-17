using Kin.KinHub.Identity.Domain.Common;
namespace Kin.KinHub.Identity.Domain.AuthenticationFeature;

public sealed class Provider : BaseActivableEntity<int>
{
    public string? Name { get; set; }
    public string? Label { get; set; }
}
