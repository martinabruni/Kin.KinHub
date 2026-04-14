using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyRepository : SqlRepository<FamilyEntity, Family, Guid>, IFamilyRepository
{
    public FamilyRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<Family?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await Set
            .FirstOrDefaultAsync(f => f.UserId == userId && !f.IsDeleted, cancellationToken);
        return entity?.Adapt<Family>();
    }
}
