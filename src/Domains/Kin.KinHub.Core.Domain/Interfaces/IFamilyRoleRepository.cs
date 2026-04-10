namespace Kin.KinHub.Core.Domain;

/// <summary>
/// Repository contract for <see cref="FamilyRole"/> configuration entities.
/// </summary>
public interface IFamilyRoleRepository : IRepository<FamilyRole, int>
{
    /// <summary>
    /// Returns the role matching the given role type, or null if not found.
    /// </summary>
    Task<FamilyRole?> FindByRoleTypeAsync(FamilyRoleType roleType, CancellationToken cancellationToken = default);
}
