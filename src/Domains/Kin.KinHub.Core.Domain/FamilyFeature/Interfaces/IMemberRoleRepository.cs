using Kin.KinHub.Core.Domain.Common;
namespace Kin.KinHub.Core.Domain.FamilyFeature;

/// <summary>
/// Repository contract for <see cref="MemberRole"/> assignments.
/// </summary>
public interface IMemberRoleRepository : IRepository<MemberRole, Guid>
{
    /// <summary>
    /// Returns all role assignments for the given member.
    /// </summary>
    Task<IReadOnlyList<MemberRole>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
}
