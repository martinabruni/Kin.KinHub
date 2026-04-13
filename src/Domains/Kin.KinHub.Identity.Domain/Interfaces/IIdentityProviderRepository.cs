using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

public interface IIdentityProviderRepository
 : IRepository<IdentityProvider, int>
{

}
