using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyServiceRepository : SqlRepository<FamilyService, Guid>, IFamilyServiceRepository
{
    public FamilyServiceRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FamilyService>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .Where(s => s.FamilyId == familyId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<FamilyService?> FindByFamilyAndServiceAsync(
        Guid familyId,
        int serviceId,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .FirstOrDefaultAsync(s => s.FamilyId == familyId && s.ServiceId == serviceId, cancellationToken);
    }
}
