using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class IdentityUserRepository : SqlRepository<IdentityUser, Guid>, IIdentityUserRepository
{
    public IdentityUserRepository(KinHubIdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IdentityUser?> FindByEmailAsync(string email)
    {
        return await Set
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(IdentityUser model)
    {
        var duplicate = await Set
            .AnyAsync(u => u.Email.ToLower() == model.Email.ToLower());

        if (duplicate)
            throw new DuplicateEntityException(nameof(IdentityUser), nameof(IdentityUser.Email), model.Email);
    }
}
