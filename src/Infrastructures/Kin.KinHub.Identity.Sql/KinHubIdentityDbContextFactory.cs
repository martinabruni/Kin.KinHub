using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kin.KinHub.Identity.Sql;

public sealed class KinHubIdentityDbContextFactory : IDesignTimeDbContextFactory<KinHubIdentityDbContext>
{
    public KinHubIdentityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<KinHubIdentityDbContext>()
            .UseSqlServer("Server=.;Database=KinHub;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new KinHubIdentityDbContext(options);
    }
}
