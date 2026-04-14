using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.PostgreSql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.PostgreSql;

public sealed class UserProviderRepository
    : PostgreSqlRepository<UserProviderEntity, UserProvider, Guid>, IUserProviderRepository
{
    public UserProviderRepository(IdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(UserProviderEntity entity)
    {
        var duplicate = await Set
            .AnyAsync(x => x.UserId == entity.UserId && x.ProviderId == entity.ProviderId);

        if (duplicate)
            throw new DuplicateEntityException(
                nameof(UserProvider),
                $"{nameof(UserProviderEntity.UserId)}/{nameof(UserProviderEntity.ProviderId)}",
                $"{entity.UserId}/{entity.ProviderId}");
    }
}
