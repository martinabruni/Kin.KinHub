using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class IdentityUserCredentialRepository
    : SqlRepository<IdentityUserCredential, Guid>, IIdentityUserCredentialRepository
{
    public IdentityUserCredentialRepository(KinHubIdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<IdentityUserCredential?> GetByUserIdAsync(Guid userId)
    {
        return await Set.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(IdentityUserCredential model)
    {
        var duplicate = await Set.AnyAsync(x => x.UserId == model.UserId);

        if (duplicate)
            throw new DuplicateEntityException(
                nameof(IdentityUserCredential),
                nameof(IdentityUserCredential.UserId),
                model.UserId);
    }
}
