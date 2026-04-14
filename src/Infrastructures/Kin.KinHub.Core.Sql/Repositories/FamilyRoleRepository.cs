using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyRoleRepository : SqlRepository<FamilyRole, int>, IFamilyRoleRepository
{
    public FamilyRoleRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<FamilyRole?> FindByRoleTypeAsync(
        FamilyRoleType roleType,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .FirstOrDefaultAsync(r => r.Id == (int)roleType, cancellationToken);
    }
}
