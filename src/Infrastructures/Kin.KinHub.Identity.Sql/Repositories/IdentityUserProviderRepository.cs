using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class IdentityUserProviderRepository
    : SqlRepository<IdentityUserProvider, Guid>, IIdentityUserProviderRepository
{
    public IdentityUserProviderRepository(KinHubIdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    protected override async Task OnBeforeCreateAsync(IdentityUserProvider model)
    {
        var duplicate = await Set
            .AnyAsync(x => x.UserId == model.UserId && x.ProviderId == model.ProviderId);

        if (duplicate)
            throw new DuplicateEntityException(
                nameof(IdentityUserProvider),
                $"{nameof(IdentityUserProvider.UserId)}/{nameof(IdentityUserProvider.ProviderId)}",
                $"{model.UserId}/{model.ProviderId}");
    }
}
