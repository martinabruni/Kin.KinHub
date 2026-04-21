
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers JWT token generation and validation services.
    /// </summary>
    public static IServiceCollection AddKinHubIdentityJwtInfrastructure(
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
