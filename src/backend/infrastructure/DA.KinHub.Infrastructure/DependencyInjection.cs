using DA.KinHub.Domain.Families;
using DA.KinHub.Domain.Documents;
using DA.KinHub.Domain.Identity;
using DA.KinHub.Domain.KinList;
using DA.KinHub.Domain.KinServices;
using DA.KinHub.Infrastructure.Persistence;
using DA.KinHub.Infrastructure.Pagination;
using DA.KinHub.Infrastructure.Storage;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DA.KinHub.Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringMode = "ConnectionString";
    private const string ManagedIdentityMode = "ManagedIdentity";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>().Bind(configuration.GetSection(DatabaseOptions.SectionName)).ValidateOnStart();
        services.AddOptions<BlobStorageOptions>().Bind(configuration.GetSection(BlobStorageOptions.SectionName)).ValidateOnStart();
        services.AddOptions<FamilyInvitationCodeOptions>().Bind(configuration.GetSection(FamilyInvitationCodeOptions.SectionName)).ValidateOnStart();
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
        services.AddSingleton<IValidateOptions<BlobStorageOptions>, BlobStorageOptionsValidator>();
        services.AddSingleton<IValidateOptions<FamilyInvitationCodeOptions>, FamilyInvitationCodeOptionsValidator>();

        services.TryAddSingleton<TokenCredential>(provider =>
        {
            var environment = provider.GetService<IHostEnvironment>();
            return environment?.IsDevelopment() == true
                ? new DefaultAzureCredential()
                : new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
        });

        services.AddSingleton(sp => new SqlConnectionAccessTokenInterceptor(
            sp.GetRequiredService<IOptions<DatabaseOptions>>().Value,
            sp.GetRequiredService<TokenCredential>()));
        services.AddSingleton(sp => CreateApplicationContainerClient(
            sp.GetRequiredService<IOptions<BlobStorageOptions>>().Value,
            sp.GetRequiredService<TokenCredential>()));
        services.AddDataProtection()
            .SetApplicationName("KinHub")
            .PersistKeysToAzureBlobStorage(sp => sp.GetRequiredService<BlobContainerClient>().GetBlobClient("data-protection/kinhub-keyring.xml"));
        services.AddSingleton<IDocumentStorage, BlobDocumentStorage>();
        services.AddDbContext<KinHubDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseSqlServer(CreateConnectionString(databaseOptions), sqlServer =>
            {
                sqlServer.CommandTimeout(databaseOptions.CommandTimeoutSeconds);
                sqlServer.MigrationsAssembly(typeof(KinHubDbContext).Assembly.FullName);
                sqlServer.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(serviceProvider.GetRequiredService<SqlConnectionAccessTokenInterceptor>());
        });
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IFamilyRepository, FamilyRepository>();
        services.AddScoped<IFamilyMembershipRepository, FamilyMembershipRepository>();
        services.AddScoped<IFamilyInvitationRepository, FamilyInvitationRepository>();
        services.AddScoped<IFamilyDetailsRepository, FamilyDetailsRepository>();
        services.AddScoped<IFamilyMemberPageRepository, FamilyMemberPageRepository>();
        services.AddScoped<IFamilyInvitationPageRepository, FamilyInvitationPageRepository>();
        services.AddScoped<IActiveKinListItemRepository, ActiveKinListItemRepository>();
        services.AddScoped<IKinServiceRepository, KinServiceRepository>();
        services.AddSingleton<IActiveItemsCursorCodec, ActiveItemsCursorCodec>();
        services.AddSingleton<IFamilyInvitationCodeProtector, FamilyInvitationCodeProtector>();
        services.AddSingleton<IFamilyMemberCursorCodec, FamilyMemberCursorCodec>();
        services.AddSingleton<IFamilyInvitationCursorCodec, FamilyInvitationCursorCodec>();
        services.AddHealthChecks().AddDbContextCheck<KinHubDbContext>("database", tags: [InfrastructureHealthChecks.ReadyTag]);
        services.AddHostedService<DatabaseMigrationHostedService>();
        return services;
    }

    private static BlobContainerClient CreateApplicationContainerClient(BlobStorageOptions options, TokenCredential credential)
    {
        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            return new BlobContainerClient(options.ConnectionString, options.ContainerName);
        }

        var containerUri = new Uri(new Uri(options.AccountUri.TrimEnd('/') + "/", UriKind.Absolute), options.ContainerName);
        return new BlobContainerClient(containerUri, credential);
    }

    private static string CreateConnectionString(DatabaseOptions options)
    {
        if (string.Equals(options.Mode, ConnectionStringMode, StringComparison.Ordinal))
        {
            return options.ConnectionString ?? throw new InvalidOperationException("Database connection string is required.");
        }

        if (!string.Equals(options.Mode, ManagedIdentityMode, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported database mode '{options.Mode}'.");
        }

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = $"tcp:{options.Host},{options.Port}",
            InitialCatalog = options.DatabaseName,
            Encrypt = options.RequireSsl,
            TrustServerCertificate = false,
            ConnectTimeout = options.CommandTimeoutSeconds
        };

        return builder.ConnectionString;
    }
}
