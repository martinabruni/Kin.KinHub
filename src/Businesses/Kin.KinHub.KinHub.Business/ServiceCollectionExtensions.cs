using Kin.KinHub.KinHub.Business.Auth;
using Kin.KinHub.KinHub.Business.Common;

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
