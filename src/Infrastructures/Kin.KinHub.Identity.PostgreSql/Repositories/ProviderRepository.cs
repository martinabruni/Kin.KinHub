using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.PostgreSql.Models;

namespace Kin.KinHub.Identity.PostgreSql;

public sealed class ProviderRepository : PostgreSqlRepository<ProviderEntity, Provider, int>, IProviderRepository
{
    public ProviderRepository(IdentityDbContext context)
        : base(context) { }
}
