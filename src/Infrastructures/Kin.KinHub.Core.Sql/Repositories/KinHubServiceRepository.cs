using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class KinHubServiceRepository : SqlRepository<KinHubService, int>, IKinHubServiceRepository
{
    public KinHubServiceRepository(KinHubCoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<KinHubService?> FindByServiceTypeAsync(
        KinHubServiceType serviceType,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .FirstOrDefaultAsync(s => s.Id == (int)serviceType, cancellationToken);
    }
}
