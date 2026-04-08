using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Jwt;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers JWT token generation and validation services.
    /// </summary>
    public static IServiceCollection AddJwtInfrastructure(
        this IServiceCollection services,
        Action<JwtOptions> configure)
    {
        var options = new JwtOptions();
        configure(options);
        options.Validate();

        var generator = new JwtTokenGenerator(options);

        services.AddSingleton<ITokenGenerator>(generator);
        services.AddSingleton<ITokenValidator>(generator);

        return services;
    }
}
