using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class FamilyMemberRepository : SqlRepository<FamilyMemberEntity, FamilyMember, Guid>, IFamilyMemberRepository
{
    public FamilyMemberRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<FamilyMember>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken = default)
    {
        var entities = await Set
            .Where(m => m.FamilyId == familyId && !m.IsDeleted)
            .ToListAsync(cancellationToken);
        return entities.Adapt<IReadOnlyList<FamilyMember>>();
    }

    /// <inheritdoc/>
    public async Task<FamilyMember?> FindByNameAsync(
        Guid familyId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var entity = await Set
            .FirstOrDefaultAsync(
                m => m.FamilyId == familyId
                     && m.Name.ToLower() == name.ToLower()
                     && !m.IsDeleted,
                cancellationToken);
        return entity?.Adapt<FamilyMember>();
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(FamilyMemberEntity entity)
    {
        var duplicate = await Set
            .AnyAsync(m =>
                m.FamilyId == entity.FamilyId
                && m.Name.ToLower() == entity.Name.ToLower()
                && !m.IsDeleted);

        if (duplicate)
            throw new DuplicateEntityException(nameof(FamilyMember), nameof(FamilyMemberEntity.Name), entity.Name);
    }
}
