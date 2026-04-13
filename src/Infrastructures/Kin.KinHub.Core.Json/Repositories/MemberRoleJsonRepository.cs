using Kin.KinHub.Core.Domain;

namespace Kin.KinHub.Core.Json;

public sealed class MemberRoleJsonRepository : JsonRepository<MemberRole, Guid>, IMemberRoleRepository
{
    public MemberRoleJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "memberRoles.json")) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<MemberRole>> GetByMemberIdAsync(
        Guid memberId,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items
            .Where(r => r.MemberId == memberId)
            .ToList();
    }
}
