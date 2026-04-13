using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Sql;
using Kin.KinHub.Identity.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKinHubIdentitySqlInfrastructure(
        this IServiceCollection services,
        Action<SqlInfrastructureOptions> configure)
    {
        var options = new SqlInfrastructureOptions();
        configure(options);
        options.Validate();

        services.AddDbContext<KinHubIdentityDbContext>(o =>
            o.UseSqlServer(options.ConnectionString));

        services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
        services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();
        services.AddScoped<IIdentityProviderRepository, IdentityProviderRepository>();
        services.AddScoped<IIdentityUserProviderRepository, IdentityUserProviderRepository>();
        services.AddScoped<IIdentityUserCredentialRepository, IdentityUserCredentialRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
