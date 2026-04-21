using Kin.KinHub.Identity.Domain.Common;

namespace Kin.KinHub.Identity.Domain.AuthenticationFeature;

public interface IUserProviderRepository
 : IRepository<UserProvider, Guid>
{

}
