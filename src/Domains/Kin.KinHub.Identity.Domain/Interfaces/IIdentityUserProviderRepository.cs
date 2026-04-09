using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

public interface IIdentityUserProviderRepository
 : IRepository<IdentityUserProvider, Guid>
{

}
