using Kin.KinHub.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyRepository : SqlRepository<Family, Guid>, IFamilyRepository
{
    public FamilyRepository(KinHubCoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<Family?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .FirstOrDefaultAsync(f => f.UserId == userId && !f.IsDeleted, cancellationToken);
    }
}
