using System.Linq;
using Azure.Core;
using DA.KinHub.Domain.Families;
using DA.KinHub.Domain.Identity;
using DA.KinHub.Infrastructure;
using DA.KinHub.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace DA.KinHub.IntegrationTests;

public sealed class FamilyRepositorySqlServerTests
{
    [SkippableFact]
    public async Task MigrateAppliesSchemasBinaryColumnAndCatalogSeed()
    {
        await using var harness = await SqlServerIntegrationTestHarness.CreateAsync();

        await harness.MigrateAsync();

        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'shared';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'kinlist';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'shared' AND TABLE_NAME = 'families' AND COLUMN_NAME = 'name';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'shared' AND TABLE_NAME = 'family_invitations';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'shared' AND TABLE_NAME = 'family_invitations' AND COLUMN_NAME = 'code_hmac' AND DATA_TYPE = 'varbinary';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM sys.indexes WHERE object_id = OBJECT_ID('shared.family_invitations') AND name = 'IX_family_invitations_active_by_family_created_at_id';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM sys.indexes WHERE object_id = OBJECT_ID('shared.family_memberships') AND name = 'IX_family_memberships_single_active_user';"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.kin_services WHERE [key] = 'kinlist' AND route = '/kinlist' AND is_active = 1 AND is_preconfigured = 1;"));
        Assert.Equal(2L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.kin_service_localizations WHERE kin_service_id = (SELECT [Id] FROM shared.kin_services WHERE [key] = 'kinlist');"));
    }

    [SkippableFact]
    public async Task CreateWithCreatorAsyncCreatesFamilyAndRetryReturnsExistingFamily()
    {
        await using var harness = await SqlServerIntegrationTestHarness.CreateAsync();

        await harness.MigrateAsync();
        var user = await harness.SeedUserAsync();

        var created = await harness.CreateFamilyAsync(user.Id, "Famiglia Bruni");
        var retried = await harness.CreateFamilyAsync(user.Id, "Nuovo Nome Ignorato");

        var persistedName = await harness.ExecuteScalarAsync<string>("SELECT TOP 1 name FROM shared.families;");
        var orphanCount = await harness.ExecuteScalarAsync<long>(
            """
            SELECT COUNT(*)
            FROM shared.families f
            LEFT JOIN shared.family_memberships fm ON fm.family_id = f.[Id] AND fm.inactive_at IS NULL
            WHERE fm.[Id] IS NULL;
            """);

        var createdResult = Assert.IsType<FamilyCreationPersistenceResult.Created>(created);
        var existingResult = Assert.IsType<FamilyCreationPersistenceResult.Existing>(retried);
        Assert.Equal(createdResult.FamilyId, existingResult.FamilyId);
        Assert.False(existingResult.ReconciledConflict);
        Assert.Equal("Famiglia Bruni", persistedName);
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.families;"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.family_memberships;"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.family_kin_service_availabilities;"));
        Assert.Equal(0L, orphanCount);
    }

    [SkippableFact]
    public async Task CreateWithCreatorAsyncConcurrentRequestsCreateSingleFamilyWithoutOrphans()
    {
        await using var harness = await SqlServerIntegrationTestHarness.CreateAsync();

        await harness.MigrateAsync();
        var user = await harness.SeedUserAsync();
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var firstAttempt = Task.Run(async () =>
        {
            await gate.Task;
            return await harness.CreateFamilyAsync(user.Id, "Famiglia Bruni Uno");
        });

        var secondAttempt = Task.Run(async () =>
        {
            await gate.Task;
            return await harness.CreateFamilyAsync(user.Id, "Famiglia Bruni Due");
        });

        gate.SetResult();
        var results = await Task.WhenAll(firstAttempt, secondAttempt);

        Assert.Single(results, result => result is FamilyCreationPersistenceResult.Created);
        Assert.Single(results, result => result is FamilyCreationPersistenceResult.Existing);
        Assert.Single(results.Select(result => result.FamilyId).Distinct());
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.families;"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.family_memberships;"));
        Assert.Equal(1L, await harness.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM shared.family_kin_service_availabilities;"));
        Assert.Equal(0L, await harness.ExecuteScalarAsync<long>(
            """
            SELECT COUNT(*)
            FROM shared.families f
            LEFT JOIN shared.family_memberships fm ON fm.family_id = f.[Id] AND fm.inactive_at IS NULL
            WHERE fm.[Id] IS NULL;
            """));
    }

    private sealed class SqlServerIntegrationTestHarness : IAsyncDisposable
    {
        private readonly MsSqlContainer? container;
        private readonly string administrativeConnectionString;
        private readonly ServiceProvider serviceProvider;
        private readonly string databaseName;

        private SqlServerIntegrationTestHarness(
            MsSqlContainer? container,
            string administrativeConnectionString,
            string connectionString,
            string databaseName,
            ServiceProvider serviceProvider)
        {
            this.container = container;
            this.administrativeConnectionString = administrativeConnectionString;
            ConnectionString = connectionString;
            this.databaseName = databaseName;
            this.serviceProvider = serviceProvider;
        }

        public string ConnectionString { get; }

        public static async Task<SqlServerIntegrationTestHarness> CreateAsync()
        {
            var explicitConnectionString = Environment.GetEnvironmentVariable("KINHUB_TEST_SQLSERVER_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(explicitConnectionString))
            {
                var provisionedDatabase = await ProvisionDatabaseAsync(explicitConnectionString);
                var provider = CreateServiceProvider(provisionedDatabase.ConnectionString);
                return new SqlServerIntegrationTestHarness(
                    null,
                    provisionedDatabase.AdministrativeConnectionString,
                    provisionedDatabase.ConnectionString,
                    provisionedDatabase.DatabaseName,
                    provider);
            }

            Skip.IfNot(IsDockerAvailable(), "Docker non disponibile e KINHUB_TEST_SQLSERVER_CONNECTION_STRING non configurata.");

            var container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();
            await container.StartAsync();

            var containerDatabase = await ProvisionDatabaseAsync(container.GetConnectionString());
            var serviceProvider = CreateServiceProvider(containerDatabase.ConnectionString);
            return new SqlServerIntegrationTestHarness(
                container,
                containerDatabase.AdministrativeConnectionString,
                containerDatabase.ConnectionString,
                containerDatabase.DatabaseName,
                serviceProvider);
        }

        public async Task MigrateAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KinHubDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        public async Task<ApplicationUser> SeedUserAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<KinHubDbContext>();
            var user = ApplicationUser.Create(new ExternalIdentity("https://issuer", Guid.NewGuid()), DateTimeOffset.UtcNow);
            dbContext.ApplicationUsers.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<FamilyCreationPersistenceResult> CreateFamilyAsync(Guid applicationUserId, string familyName)
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IFamilyRepository>();
            var now = DateTimeOffset.UtcNow;
            var family = Family.Create(FamilyName.Create(familyName), applicationUserId, now);
            var membership = FamilyMembership.Create(applicationUserId, family.Id, now);
            return await repository.CreateWithCreatorAsync(applicationUserId, family, membership, CancellationToken.None);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql)
        {
            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            var result = await command.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result ?? throw new InvalidOperationException("Expected a scalar result."), typeof(T));
        }

        public async ValueTask DisposeAsync()
        {
            await serviceProvider.DisposeAsync();

            try
            {
                await DropDatabaseAsync(administrativeConnectionString, databaseName);
            }
            finally
            {
                if (container is not null)
                {
                    await container.DisposeAsync();
                }
            }
        }

        private static ServiceProvider CreateServiceProvider(string connectionString)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Mode"] = "ConnectionString",
                ["Database:ConnectionString"] = connectionString,
                ["Database:ApplyMigrationsOnStartup"] = "false",
                ["Storage:AccountUri"] = "https://kinhubtest.blob.core.windows.net/",
                ["Storage:ContainerName"] = "documents"
            }).Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging();
            services.AddSingleton<IHostEnvironment>(new HostingEnvironmentStub(isDevelopment: true));
            services.AddSingleton<TokenCredential>(new StaticTokenCredential());
            services.AddInfrastructure(configuration);
            return services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }

        private static bool IsDockerAvailable()
            => File.Exists("\\\\.\\pipe\\dockerDesktopLinuxEngine") || File.Exists("\\\\.\\pipe\\docker_engine");

        private static async Task<(string AdministrativeConnectionString, string ConnectionString, string DatabaseName)> ProvisionDatabaseAsync(string baseConnectionString)
        {
            var databaseName = $"kinhub_{Guid.NewGuid():N}";
            var administrativeBuilder = new SqlConnectionStringBuilder(baseConnectionString)
            {
                InitialCatalog = "master",
                Pooling = false,
                TrustServerCertificate = true
            };

            await using (var connection = new SqlConnection(administrativeBuilder.ConnectionString))
            {
                await connection.OpenAsync();
                await using var command = new SqlCommand($"CREATE DATABASE [{databaseName}];", connection);
                await command.ExecuteNonQueryAsync();
            }

            var builder = new SqlConnectionStringBuilder(administrativeBuilder.ConnectionString)
            {
                InitialCatalog = databaseName,
                Pooling = false
            };

            return (administrativeBuilder.ConnectionString, builder.ConnectionString, databaseName);
        }

        private static async Task DropDatabaseAsync(string administrativeConnectionString, string databaseName)
        {
            await using var connection = new SqlConnection(administrativeConnectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE IF EXISTS [{databaseName}];", connection);
            await command.ExecuteNonQueryAsync();
        }

        private sealed class StaticTokenCredential : TokenCredential
        {
            public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
                => new("unused", DateTimeOffset.MaxValue);

            public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
                => ValueTask.FromResult(GetToken(requestContext, cancellationToken));
        }

        private sealed class HostingEnvironmentStub(bool isDevelopment) : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = isDevelopment ? Environments.Development : Environments.Production;

            public string ApplicationName { get; set; } = "KinHub.Tests";

            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
