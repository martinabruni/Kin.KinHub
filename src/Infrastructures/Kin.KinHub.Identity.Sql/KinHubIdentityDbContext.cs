using Kin.KinHub.Identity.Domain.Enums;
using Kin.KinHub.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public sealed class KinHubIdentityDbContext : DbContext
{
    private static readonly DateTime _seedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public KinHubIdentityDbContext(DbContextOptions<KinHubIdentityDbContext> options)
        : base(options) { }

    public DbSet<IdentityUser> IdentityUsers => Set<IdentityUser>();
    public DbSet<IdentityUserCredential> IdentityUserCredentials => Set<IdentityUserCredential>();
    public DbSet<IdentityProvider> IdentityProviders => Set<IdentityProvider>();
    public DbSet<IdentityUserProvider> IdentityUserProviders => Set<IdentityUserProvider>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<IdentityUser>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.DisplayName).HasMaxLength(200);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<IdentityUserCredential>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.UserId).IsUnique();
        });

        modelBuilder.Entity<IdentityProvider>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(100);
            e.Property(p => p.Label).HasMaxLength(200);
            e.HasData(
                new IdentityProvider { Id = (int)IdentityProviderType.KinHub, Name = "kinhub", Label = "Accedi con KinHub", IsActive = true, CreatedAt = _seedDate, UpdatedAt = _seedDate },
                new IdentityProvider { Id = (int)IdentityProviderType.Google, Name = "google", Label = "Accedi con Google", IsActive = false, CreatedAt = _seedDate, UpdatedAt = _seedDate },
                new IdentityProvider { Id = (int)IdentityProviderType.GitHub, Name = "github", Label = "Accedi con GitHub", IsActive = false, CreatedAt = _seedDate, UpdatedAt = _seedDate },
                new IdentityProvider { Id = (int)IdentityProviderType.Microsoft, Name = "microsoft", Label = "Accedi con Microsoft", IsActive = false, CreatedAt = _seedDate, UpdatedAt = _seedDate });
        });

        modelBuilder.Entity<IdentityUserProvider>(e =>
        {
            e.HasKey(up => up.Id);
            e.Property(up => up.ProviderUserId).IsRequired().HasMaxLength(256);
            e.HasIndex(up => new { up.UserId, up.ProviderId }).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Token).IsRequired().HasMaxLength(512);
            e.HasIndex(t => t.Token).IsUnique();
            e.HasIndex(t => t.UserId);
        });
    }
}
