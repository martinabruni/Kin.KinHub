using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class KinHubServiceRepository : SqlRepository<KinHubServiceEntity, KinHubService, int>, IKinHubServiceRepository
{
    public KinHubServiceRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<KinHubService?> FindByServiceTypeAsync(
        KinHubServiceType serviceType,
        CancellationToken cancellationToken = default)
    {
        var entity = await Set
            .FirstOrDefaultAsync(s => s.Id == (int)serviceType, cancellationToken);
        return entity?.Adapt<KinHubService>();
    }
}
