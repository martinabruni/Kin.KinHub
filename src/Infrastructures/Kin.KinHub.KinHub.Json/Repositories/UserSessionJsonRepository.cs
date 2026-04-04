using Kin.KinHub.KinHub.Domain.Interfaces;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Json;

public sealed class UserSessionJsonRepository : JsonRepository<UserSession, Guid>, IUserSessionRepository
{
    public UserSessionJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "userSessions.json")) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<UserSession>> GetByUserIdAsync(Guid userId)
    {
        var items = await ReadAllAsync();
        return items.Where(s => s.UserId == userId).ToList();
    }
}
