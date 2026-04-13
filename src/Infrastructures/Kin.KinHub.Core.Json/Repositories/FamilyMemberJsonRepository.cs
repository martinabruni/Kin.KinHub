using Kin.KinHub.Core.Domain;

namespace Kin.KinHub.Core.Json;

public sealed class FamilyMemberJsonRepository : JsonRepository<FamilyMember, Guid>, IFamilyMemberRepository
{
    public FamilyMemberJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "familyMembers.json")) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FamilyMember>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items
            .Where(m => m.FamilyId == familyId && !m.IsDeleted)
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<FamilyMember?> FindByNameAsync(
        Guid familyId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(m =>
            m.FamilyId == familyId
            && string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase)
            && !m.IsDeleted);
    }

    /// <inheritdoc/>
    protected override Task OnBeforeCreateAsync(List<FamilyMember> existingItems, FamilyMember newItem)
    {
        var duplicate = existingItems.FirstOrDefault(m =>
            m.FamilyId == newItem.FamilyId
            && string.Equals(m.Name, newItem.Name, StringComparison.OrdinalIgnoreCase)
            && !m.IsDeleted);

        if (duplicate is not null)
            throw new DuplicateEntityException(nameof(FamilyMember), nameof(FamilyMember.Name), newItem.Name);

        return Task.CompletedTask;
    }
}
