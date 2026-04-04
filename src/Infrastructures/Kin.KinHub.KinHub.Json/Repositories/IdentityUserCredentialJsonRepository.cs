using Kin.KinHub.KinHub.Domain;
using Kin.KinHub.KinHub.Domain.Interfaces;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Json;

public sealed class IdentityUserCredentialJsonRepository
    : JsonRepository<IdentityUserCredential, Guid>, IIdentityUserCredentialRepository
{
    public IdentityUserCredentialJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "identityUserCredentials.json")) { }

    /// <inheritdoc/>
    protected override Task OnBeforeCreateAsync(
        List<IdentityUserCredential> existingItems,
        IdentityUserCredential newItem)
    {
        var duplicate = existingItems.FirstOrDefault(x => x.UserId == newItem.UserId);

        if (duplicate is not null)
            throw new DuplicateEntityException(
                nameof(IdentityUserCredential),
                nameof(IdentityUserCredential.UserId),
                newItem.UserId);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<IdentityUserCredential?> GetByUserIdAsync(Guid userId)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(x => x.UserId == userId);
    }
}
