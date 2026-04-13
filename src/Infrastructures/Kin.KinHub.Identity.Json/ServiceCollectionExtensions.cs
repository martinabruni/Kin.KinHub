using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Json;
using Kin.KinHub.Identity.Json.Models;
using Kin.KinHub.Identity.Json.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the JSON-based infrastructure repositories.
    /// </summary>
    public static IServiceCollection AddKinHubJsonInfrastructure(
        this IServiceCollection services,
        Action<JsonInfrastructureOptions> configure)
    {
        var options = new JsonInfrastructureOptions();
        configure(options);
        options.Validate();

        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        services.AddSingleton<IIdentityUserRepository>(_ =>
            new IdentityUserJsonRepository(options.DataDirectory));

        services.AddSingleton<IIdentityProviderRepository>(_ =>
            new IdentityProviderJsonRepository(options.DataDirectory));

        services.AddSingleton<IIdentityUserProviderRepository>(_ =>
            new IdentityUserProviderJsonRepository(options.DataDirectory));

        services.AddSingleton<IIdentityUserCredentialRepository>(_ =>
            new IdentityUserCredentialJsonRepository(options.DataDirectory));

        services.AddSingleton<IRefreshTokenRepository>(_ =>
            new RefreshTokenJsonRepository(options.DataDirectory));

        return services;
    }
}
