using Kin.KinHub.Core.Domain;

namespace Kin.KinHub.Core.Json;

public sealed class FamilyServiceJsonRepository : JsonRepository<FamilyService, Guid>, IFamilyServiceRepository
{
    public FamilyServiceJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "familyServices.json")) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FamilyService>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items.Where(s => s.FamilyId == familyId).ToList();
    }

    /// <inheritdoc/>
    public async Task<FamilyService?> FindByFamilyAndServiceAsync(
        Guid familyId,
        int serviceId,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(s => s.FamilyId == familyId && s.ServiceId == serviceId);
    }
}
