using Kin.KinHub.KinHub.Domain.Interfaces;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Json;

public sealed class IdentityProviderJsonRepository : JsonRepository<IdentityProvider, int>, IIdentityProviderRepository
{
    public IdentityProviderJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "identityProviders.json")) { }
}
