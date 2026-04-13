using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;
using Kin.KinHub.Identity.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class RefreshTokenRepository : SqlRepository<RefreshToken, Guid>, IRefreshTokenRepository
{
    public RefreshTokenRepository(KinHubIdentityDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        return await Set
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    /// <inheritdoc/>
    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var tokens = await Set
            .Where(x => x.UserId == userId && !x.Revoked)
            .ToListAsync();

        if (tokens.Count == 0)
            return;

        foreach (var token in tokens)
            token.Revoked = true;

        await Context.SaveChangesAsync();
    }
}
