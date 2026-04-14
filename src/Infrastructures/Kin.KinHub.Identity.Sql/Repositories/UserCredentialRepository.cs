using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class UserCredentialRepository
    : SqlRepository<UserCredential, Guid>, IUserCredentialRepository
{
    public UserCredentialRepository(IdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<UserCredential?> GetByUserIdAsync(Guid userId)
    {
        return await Set.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(UserCredential model)
    {
        var duplicate = await Set.AnyAsync(x => x.UserId == model.UserId);

        if (duplicate)
            throw new DuplicateEntityException(
                nameof(UserCredential),
                nameof(UserCredential.UserId),
                model.UserId);
    }
}
