using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DA.KinHub.Infrastructure.Persistence;

public sealed class KinHubDbContextFactory : IDesignTimeDbContextFactory<KinHubDbContext>
{
    public KinHubDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
            ?? "Server=localhost,1433;Database=kinhub;User Id=sa;Password=LocalDevPassword123!;TrustServerCertificate=True;Encrypt=False";
        var options = new DbContextOptionsBuilder<KinHubDbContext>()
            .UseSqlServer(connectionString, sqlServer => sqlServer.MigrationsAssembly(typeof(KinHubDbContext).Assembly.FullName))
            .Options;
        return new KinHubDbContext(options);
    }
}
