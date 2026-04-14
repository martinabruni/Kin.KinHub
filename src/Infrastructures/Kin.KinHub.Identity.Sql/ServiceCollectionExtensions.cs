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

        services.AddDbContext<IdentityDbContext>(o =>
            o.UseSqlServer(options.ConnectionString));

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IKinUserRepository, KinUserRepository>();
        services.AddScoped<IProviderRepository, ProviderRepository>();
        services.AddScoped<IUserProviderRepository, UserProviderRepository>();
        services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}
