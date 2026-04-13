using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Json;

public sealed class IdentityProviderJsonRepository : JsonRepository<IdentityProvider, int>, IIdentityProviderRepository
{
    public IdentityProviderJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "identityProviders.json")) { }
}
