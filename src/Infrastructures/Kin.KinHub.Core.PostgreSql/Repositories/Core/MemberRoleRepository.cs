using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.PostgreSql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.PostgreSql.Repositories.Core;

public sealed class MemberRoleRepository : PostgreSqlRepository<MemberRoleEntity, MemberRole, Guid>, IMemberRoleRepository
{
    public MemberRoleRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<MemberRole>> GetByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var entities = await Set
            .Where(r => r.MemberId == memberId)
            .ToListAsync(cancellationToken);
        return entities.Adapt<IReadOnlyList<MemberRole>>();
    }
}
