using Kin.KinHub.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class MemberRoleRepository : SqlRepository<MemberRole, Guid>, IMemberRoleRepository
{
    public MemberRoleRepository(KinHubCoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<MemberRole>> GetByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .Where(r => r.MemberId == memberId)
            .ToListAsync(cancellationToken);
    }
}
