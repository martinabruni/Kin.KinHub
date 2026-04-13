using Kin.KinHub.Core.Business;
using Kin.KinHub.Core.Business.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the KinHub Core business services.
    /// </summary>
    public static IServiceCollection AddKinHubCoreBusiness(
        this IServiceCollection services,
        Action<BusinessOptions>? configure = null)
    {
        var options = new BusinessOptions();
        configure?.Invoke(options);
        options.Validate();

        services.AddScoped<IFamilyService, KinHubFamilyService>();
        services.AddScoped<IKinHubServiceService, KinHubServiceService>();

        return services;
    }
}
