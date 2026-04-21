
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the KinHub business services.
    /// </summary>
    public static IServiceCollection AddKinHubIdentityBusiness(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, KinHubAuthenticationService>();

        return services;
    }
}
