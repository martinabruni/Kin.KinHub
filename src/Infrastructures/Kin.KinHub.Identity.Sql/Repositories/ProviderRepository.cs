using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;

namespace Kin.KinHub.Identity.Sql;

public sealed class ProviderRepository : SqlRepository<ProviderEntity, Provider, int>, IProviderRepository
{
    public ProviderRepository(IdentityDbContext context)
        : base(context) { }
}
