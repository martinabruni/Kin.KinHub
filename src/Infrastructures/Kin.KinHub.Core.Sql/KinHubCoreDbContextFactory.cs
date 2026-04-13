using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kin.KinHub.Core.Sql;

public sealed class KinHubCoreDbContextFactory : IDesignTimeDbContextFactory<KinHubCoreDbContext>
{
    public KinHubCoreDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<KinHubCoreDbContext>()
            .UseSqlServer("Server=.;Database=KinHub;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new KinHubCoreDbContext(options);
    }
}
