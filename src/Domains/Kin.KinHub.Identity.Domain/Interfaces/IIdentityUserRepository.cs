using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

public interface IIdentityUserRepository : IRepository<IdentityUser, Guid>
{
    /// <summary>
    /// Returns the user matching the given email, or null if not found.
    /// </summary>
    Task<IdentityUser?> FindByEmailAsync(string email);
}
