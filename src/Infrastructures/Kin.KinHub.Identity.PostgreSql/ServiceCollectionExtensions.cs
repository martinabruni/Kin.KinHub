using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.PostgreSql.Models;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKinHubIdentityPostgreSqlInfrastructure(
        this IServiceCollection services,
        Action<Kin.KinHub.Identity.PostgreSql.PostgreSqlOptions> configure)
    {
        var options = new Kin.KinHub.Identity.PostgreSql.PostgreSqlOptions();
        configure(options);
        options.Validate();

        services.AddDbContext<IdentityDbContext>(o =>
            o.UseNpgsql(options.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            }));

        services.AddScoped<IPasswordHasher, Kin.KinHub.Identity.PostgreSql.PasswordHasher>();
        services.AddScoped<IKinUserRepository, Kin.KinHub.Identity.PostgreSql.KinUserRepository>();
        services.AddScoped<IProviderRepository, Kin.KinHub.Identity.PostgreSql.ProviderRepository>();
        services.AddScoped<IUserProviderRepository, Kin.KinHub.Identity.PostgreSql.UserProviderRepository>();
        services.AddScoped<IUserCredentialRepository, Kin.KinHub.Identity.PostgreSql.UserCredentialRepository>();
        services.AddScoped<IRefreshTokenRepository, Kin.KinHub.Identity.PostgreSql.RefreshTokenRepository>();

        return services;
    }
}
