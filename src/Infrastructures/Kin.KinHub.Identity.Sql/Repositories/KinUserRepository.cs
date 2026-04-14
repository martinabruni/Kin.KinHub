using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class KinUserRepository : SqlRepository<KinUserEntity, KinUser, Guid>, IKinUserRepository
{
    public KinUserRepository(IdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<KinUser?> FindByEmailAsync(string email)
    {
        var entity = await Set
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        return entity?.Adapt<KinUser>();
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(KinUserEntity entity)
    {
        var duplicate = await Set
            .AnyAsync(u => u.Email.ToLower() == entity.Email.ToLower());

        if (duplicate)
            throw new DuplicateEntityException(nameof(KinUser), nameof(KinUserEntity.Email), entity.Email);
    }
}
