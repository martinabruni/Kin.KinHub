using Kin.KinHub.Core.Domain.Common;
namespace Kin.KinHub.Core.Domain.FamilyFeature;

/// <summary>
/// Repository contract for <see cref="Family"/> aggregates.
/// </summary>
public interface IFamilyRepository : IRepository<Family, Guid>
{
    /// <summary>
    /// Returns the family owned by the given user, or null if not found.
    /// </summary>
    Task<Family?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
