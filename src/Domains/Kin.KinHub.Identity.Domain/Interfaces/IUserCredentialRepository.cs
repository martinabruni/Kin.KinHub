using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

public interface IUserCredentialRepository : IRepository<UserCredential, Guid>
{
    /// <summary>
    /// Returns the credential for the given user, or null if not found.
    /// </summary>
    Task<UserCredential?> GetByUserIdAsync(Guid userId);
}
