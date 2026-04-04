using Kin.KinHub.KinHub.Domain;
using Kin.KinHub.KinHub.Domain.Interfaces;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Json;

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
