using Kin.KinHub.KinHub.Domain;

namespace Kin.KinHub.KinHub.Json;

public sealed class RoleJsonRepository : JsonRepository<Role, int>, IRoleRepository
{
    public RoleJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "roles.json")) { }
}
