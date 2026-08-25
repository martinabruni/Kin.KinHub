using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;

namespace DA.KinHub.Infrastructure.Persistence;

internal sealed class DatabaseMigrationHostedService(
    IServiceProvider serviceProvider,
    IHostEnvironment environment,
    IOptions<DatabaseOptions> options,
    ILogger<DatabaseMigrationHostedService> logger) : IHostedService
{
    private const string MigrationLockName = "kinhub-local-migrations";
    private const int LockTimeoutMilliseconds = 30_000;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.ApplyMigrationsOnStartup)
        {
            return;
        }

        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException("Startup migrations are allowed only in Development. Use the CI migration bundle in other environments.");
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<KinHubDbContext>();
        logger.LogInformation("Acquiring SQL Server application lock before local migrations");
        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            await AcquireApplicationLockAsync(dbContext, cancellationToken);
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        finally
        {
            await ReleaseApplicationLockAsync(dbContext);
            await dbContext.Database.CloseConnectionAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task AcquireApplicationLockAsync(KinHubDbContext dbContext, CancellationToken cancellationToken)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "EXEC @result = sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Session', @LockTimeout = @timeout; SELECT @result;";

        var resultParameter = new SqlParameter("@result", System.Data.SqlDbType.Int)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        command.Parameters.Add(resultParameter);
        command.Parameters.Add(new SqlParameter("@resource", MigrationLockName));
        command.Parameters.Add(new SqlParameter("@timeout", LockTimeoutMilliseconds));

        await command.ExecuteNonQueryAsync(cancellationToken);
        if (resultParameter.Value is not int result || result < 0)
        {
            throw new InvalidOperationException($"Failed to acquire SQL application lock for local migrations. Result: {resultParameter.Value ?? "null"}.");
        }
    }

    private static async Task ReleaseApplicationLockAsync(KinHubDbContext dbContext)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "EXEC sp_releaseapplock @Resource = @resource, @LockOwner = 'Session';";
        command.Parameters.Add(new SqlParameter("@resource", MigrationLockName));
        await command.ExecuteNonQueryAsync();
    }
}
