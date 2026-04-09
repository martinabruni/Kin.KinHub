using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.ModelInterfaces;
using Kin.KinHub.Identity.Jwt;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers JWT token generation and validation services.
    /// </summary>
    public static IServiceCollection AddKinHubJwtInfrastructure(
        this IServiceCollection services,
        Action<JwtOptions> configure)
    {
        var options = new JwtOptions();
        configure(options);
        options.Validate();

        var generator = new JwtTokenGenerator(options);

        services.AddSingleton<ITokenGenerator>(generator);
        services.AddSingleton<ITokenValidator>(generator);

        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());

        return services;
    }
}
