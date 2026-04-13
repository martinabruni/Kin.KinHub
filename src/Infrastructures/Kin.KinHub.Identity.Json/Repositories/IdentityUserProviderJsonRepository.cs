using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Json;

public sealed class IdentityUserProviderJsonRepository
    : JsonRepository<IdentityUserProvider, Guid>, IIdentityUserProviderRepository
{
    public IdentityUserProviderJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "identityUserProviders.json")) { }

    /// <inheritdoc/>
    protected override Task OnBeforeCreateAsync(
        List<IdentityUserProvider> existingItems,
        IdentityUserProvider newItem)
    {
        var duplicate = existingItems.FirstOrDefault(x =>
            x.UserId == newItem.UserId && x.ProviderId == newItem.ProviderId);

        if (duplicate is not null)
            throw new DuplicateEntityException(
                nameof(IdentityUserProvider),
                $"{nameof(IdentityUserProvider.UserId)}/{nameof(IdentityUserProvider.ProviderId)}",
                $"{newItem.UserId}/{newItem.ProviderId}");

        return Task.CompletedTask;
    }
}
