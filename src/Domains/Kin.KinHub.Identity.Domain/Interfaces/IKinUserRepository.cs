using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

public interface IKinUserRepository : IRepository<KinUser, Guid>
{
    /// <summary>
    /// Returns the user matching the given email, or null if not found.
    /// </summary>
    Task<KinUser?> FindByEmailAsync(string email);
}
