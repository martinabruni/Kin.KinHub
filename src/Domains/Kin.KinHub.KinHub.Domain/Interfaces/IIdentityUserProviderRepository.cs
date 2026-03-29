using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Domain.Interfaces;

public interface IIdentityUserProviderRepository
 : IRepository<IdentityUserProvider, Guid>
{

}
