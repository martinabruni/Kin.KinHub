using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class UserProviderRepository
    : SqlRepository<UserProvider, Guid>, IUserProviderRepository
{
    public UserProviderRepository(IdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(UserProvider model)
    {
        var duplicate = await Set
            .AnyAsync(x => x.UserId == model.UserId && x.ProviderId == model.ProviderId);

        if (duplicate)
            throw new DuplicateEntityException(
                nameof(UserProvider),
                $"{nameof(UserProvider.UserId)}/{nameof(UserProvider.ProviderId)}",
                $"{model.UserId}/{model.ProviderId}");
    }
}
