using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Json;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Core JSON-based infrastructure repositories.
    /// </summary>
    public static IServiceCollection AddKinHubCoreJsonInfrastructure(
        this IServiceCollection services,
        Action<JsonInfrastructureOptions> configure)
    {
        var options = new JsonInfrastructureOptions();
        configure(options);
        options.Validate();

        services.AddSingleton<IFamilyRepository>(_ =>
            new FamilyJsonRepository(options.DataDirectory));

        services.AddSingleton<IFamilyMemberRepository>(_ =>
            new FamilyMemberJsonRepository(options.DataDirectory));

        services.AddSingleton<IFamilyRoleRepository>(_ =>
            new FamilyRoleJsonRepository(options.DataDirectory));

        services.AddSingleton<IMemberRoleRepository>(_ =>
            new MemberRoleJsonRepository(options.DataDirectory));

        services.AddSingleton<IKinHubServiceRepository>(_ =>
            new KinHubServiceJsonRepository(options.DataDirectory));

        services.AddSingleton<IFamilyServiceRepository>(_ =>
            new FamilyServiceJsonRepository(options.DataDirectory));

        return services;
    }
}
