using Kin.KinHub.Identity.Domain;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Json;

public sealed class IdentityUserJsonRepository : JsonRepository<IdentityUser, Guid>, IIdentityUserRepository
{
    public IdentityUserJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "identityUsers.json")) { }

    /// <inheritdoc/>
    protected override Task OnBeforeCreateAsync(List<IdentityUser> existingItems, IdentityUser newItem)
    {
        var duplicate = existingItems.FirstOrDefault(u =>
            string.Equals(u.Email, newItem.Email, StringComparison.OrdinalIgnoreCase));

        if (duplicate is not null)
            throw new DuplicateEntityException(nameof(IdentityUser), nameof(IdentityUser.Email), newItem.Email);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<IdentityUser?> FindByEmailAsync(string email)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }
}
