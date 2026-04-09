using Kin.KinHub.Identity.Business.Configurations;
using Kin.KinHub.Identity.Business.Interfaces;
using Kin.KinHub.Identity.Business.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the KinHub business services.
    /// </summary>
    public static IServiceCollection AddKinHubBusiness(
        this IServiceCollection services,
        Action<BusinessOptions> configure)
    {
        var options = new BusinessOptions();
        configure(options);
        options.Validate();

        services.AddScoped<IAuthenticationService, KinHubAuthenticationService>();

        return services;
    }
}
