using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Sql;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKinHubCoreSqlInfrastructure(
        this IServiceCollection services,
        Action<SqlInfrastructureOptions> configure)
    {
        var options = new SqlInfrastructureOptions();
        configure(options);
        options.Validate();

        services.AddDbContext<KinHubCoreDbContext>(o =>
            o.UseSqlServer(options.ConnectionString));

        services.AddScoped<IFamilyRepository, FamilyRepository>();
        services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
        services.AddScoped<IFamilyRoleRepository, FamilyRoleRepository>();
        services.AddScoped<IMemberRoleRepository, MemberRoleRepository>();
        services.AddScoped<IKinHubServiceRepository, KinHubServiceRepository>();
        services.AddScoped<IFamilyServiceRepository, FamilyServiceRepository>();

        return services;
    }
}
