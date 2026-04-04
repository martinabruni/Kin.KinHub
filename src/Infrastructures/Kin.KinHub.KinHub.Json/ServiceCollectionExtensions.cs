using Kin.KinHub.KinHub.Domain.Interfaces;
using Kin.KinHub.KinHub.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the JSON-based infrastructure repositories.
    /// </summary>
    public static IServiceCollection AddJsonInfrastructure(
        this IServiceCollection services,
        Action<JsonInfrastructureOptions> configure)
    {
        var options = new JsonInfrastructureOptions();
        configure(options);
        options.Validate();

        services.AddSingleton<IIdentityUserRepository>(_ =>
            new IdentityUserJsonRepository(options.DataDirectory));

        services.AddSingleton<IIdentityProviderRepository>(_ =>
            new IdentityProviderJsonRepository(options.DataDirectory));

        services.AddSingleton<IIdentityUserProviderRepository>(_ =>
            new IdentityUserProviderJsonRepository(options.DataDirectory));

        services.AddSingleton<IIdentityUserCredentialRepository>(_ =>
            new IdentityUserCredentialJsonRepository(options.DataDirectory));

        services.AddSingleton<IUserSessionRepository>(_ =>
            new UserSessionJsonRepository(options.DataDirectory));

        return services;
    }
}
