using Kin.KinHub.Identity.Domain;
using Kin.KinHub.Identity.Domain.Interfaces;

namespace Kin.KinHub.Identity.Json;

/// <inheritdoc/>
public sealed class RefreshTokenJsonRepository
    : JsonRepository<RefreshToken, Guid>, IRefreshTokenRepository
{
    public RefreshTokenJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "refreshTokens.json")) { }

    /// <inheritdoc/>
    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(x =>
            string.Equals(x.Token, token, StringComparison.Ordinal));
    }

    /// <inheritdoc/>
    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var items = await ReadAllAsync();
        var changed = false;

        foreach (var item in items.Where(x => x.UserId == userId && !x.Revoked))
        {
            item.Revoked = true;
            changed = true;
        }

        if (changed)
            await WriteAllAsync(items);
    }
}
