using Kin.KinHub.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.Sql;

public sealed class KinHubCoreDbContext : DbContext
{
    private static readonly DateTime _seedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public KinHubCoreDbContext(DbContextOptions<KinHubCoreDbContext> options)
        : base(options) { }

    public DbSet<Family> Families => Set<Family>();
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<FamilyRole> FamilyRoles => Set<FamilyRole>();
    public DbSet<MemberRole> MemberRoles => Set<MemberRole>();
    public DbSet<KinHubService> KinHubServices => Set<KinHubService>();
    public DbSet<FamilyService> FamilyServices => Set<FamilyService>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("core");

        modelBuilder.Entity<Family>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Name).IsRequired().HasMaxLength(200);
            e.Property(f => f.AdminCodeHash).IsRequired();
            e.HasIndex(f => f.UserId);
        });

        modelBuilder.Entity<FamilyMember>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(m => m.FamilyId);
        });

        modelBuilder.Entity<FamilyRole>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).IsRequired().HasMaxLength(100);
            e.HasData(
                new FamilyRole { Id = (int)FamilyRoleType.Admin, Name = "admin", IsActive = true, CreatedAt = _seedDate, UpdatedAt = _seedDate },
                new FamilyRole { Id = (int)FamilyRoleType.Member, Name = "member", IsActive = true, CreatedAt = _seedDate, UpdatedAt = _seedDate });
        });

        modelBuilder.Entity<MemberRole>(e =>
        {
            e.HasKey(r => r.Id);
            e.HasIndex(r => r.MemberId);
        });

        modelBuilder.Entity<KinHubService>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Name).IsRequired().HasMaxLength(200);
            e.Property(s => s.BaseUrl).IsRequired().HasMaxLength(500);
            e.HasData(
                new KinHubService { Id = (int)KinHubServiceType.KinConsole, Name = "KinConsole", BaseUrl = "/kin-console", IsActive = true, IsAdminOnly = true, CreatedAt = _seedDate, UpdatedAt = _seedDate },
                new KinHubService { Id = (int)KinHubServiceType.KinRecipe, Name = "KinRecipe", BaseUrl = "/kin-recipe", IsActive = false, IsAdminOnly = false, CreatedAt = _seedDate, UpdatedAt = _seedDate });
        });

        modelBuilder.Entity<FamilyService>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.FamilyId);
            e.HasIndex(s => new { s.FamilyId, s.ServiceId }).IsUnique();
        });
    }
}
