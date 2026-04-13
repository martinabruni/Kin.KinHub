using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

public interface IIdentityUserCredentialRepository : IRepository<IdentityUserCredential, Guid>
{
    /// <summary>
    /// Returns the credential for the given user, or null if not found.
    /// </summary>
    Task<IdentityUserCredential?> GetByUserIdAsync(Guid userId);
}
