using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Domain.Interfaces;

public interface IIdentityUserCredentialRepository : IRepository<IdentityUserCredential, Guid>
{
    /// <summary>
    /// Returns the credential for the given user, or null if not found.
    /// </summary>
    Task<IdentityUserCredential?> GetByUserIdAsync(Guid userId);
}
