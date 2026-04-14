using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class KinUserRepository : SqlRepository<KinUser, Guid>, IKinUserRepository
{
    public KinUserRepository(IdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<KinUser?> FindByEmailAsync(string email)
    {
        return await Set
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(KinUser model)
    {
        var duplicate = await Set
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (duplicate)
            throw new DuplicateEntityException(nameof(KinUser), nameof(KinUser.Email), model.Email);
    }
}
