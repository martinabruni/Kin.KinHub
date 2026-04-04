using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Domain.Interfaces;

public interface IIdentityUserRepository : IRepository<IdentityUser, Guid>
{
    /// <summary>
    /// Returns the user matching the given email, or null if not found.
    /// </summary>
    Task<IdentityUser?> FindByEmailAsync(string email);
}
