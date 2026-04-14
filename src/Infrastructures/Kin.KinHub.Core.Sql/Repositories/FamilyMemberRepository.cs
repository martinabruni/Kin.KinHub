using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyMemberRepository : SqlRepository<FamilyMember, Guid>, IFamilyMemberRepository
{
    public FamilyMemberRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FamilyMember>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .Where(m => m.FamilyId == familyId && !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<FamilyMember?> FindByNameAsync(
        Guid familyId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await Set
            .FirstOrDefaultAsync(
                m => m.FamilyId == familyId
                     && m.Name.ToLower() == name.ToLower()
                     && !m.IsDeleted,
                cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(FamilyMember model)
    {
        var duplicate = await Set
            .AnyAsync(m =>
                m.FamilyId == model.FamilyId
                && m.Name.ToLower() == model.Name.ToLower()
                && !m.IsDeleted);

        if (duplicate)
            throw new DuplicateEntityException(nameof(FamilyMember), nameof(FamilyMember.Name), model.Name);
    }
}
