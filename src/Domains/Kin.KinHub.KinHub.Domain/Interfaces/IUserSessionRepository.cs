using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Domain.Interfaces;

public interface IUserSessionRepository : IRepository<UserSession, Guid>
{
    /// <summary>
    /// Returns all active sessions for a given user.
    /// </summary>
    Task<IReadOnlyList<UserSession>> GetByUserIdAsync(Guid userId);
}
