using Kin.KinHub.Core.Domain;

namespace Kin.KinHub.Core.Json;

public sealed class FamilyJsonRepository : JsonRepository<Family, Guid>, IFamilyRepository
{
    public FamilyJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "families.json")) { }

    /// <inheritdoc/>
    public async Task<Family?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(f => f.UserId == userId && !f.IsDeleted);
    }
}
