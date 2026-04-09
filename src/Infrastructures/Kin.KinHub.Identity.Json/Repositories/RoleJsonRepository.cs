using Kin.KinHub.Identity.Domain;

namespace Kin.KinHub.Identity.Json;

public sealed class RoleJsonRepository : JsonRepository<Role, int>, IRoleRepository
{
    public RoleJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "roles.json")) { }
}
