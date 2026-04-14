using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyRoleRepository : SqlRepository<FamilyRoleEntity, FamilyRole, int>, IFamilyRoleRepository
{
    public FamilyRoleRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<FamilyRole?> FindByRoleTypeAsync(
        FamilyRoleType roleType,
        CancellationToken cancellationToken = default)
    {
        var entity = await Set
            .FirstOrDefaultAsync(r => r.Id == (int)roleType, cancellationToken);
        return entity?.Adapt<FamilyRole>();
    }
}
