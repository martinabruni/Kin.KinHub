using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;

namespace Kin.KinHub.Identity.Sql;

public sealed class IdentityProviderRepository : SqlRepository<IdentityProvider, int>, IIdentityProviderRepository
{
    public IdentityProviderRepository(KinHubIdentityDbContext context)
        : base(context) { }
}
