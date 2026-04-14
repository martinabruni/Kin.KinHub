using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Domain.Interfaces.Recipes;
using Kin.KinHub.Core.PostgreSql.Models;
using Kin.KinHub.Core.PostgreSql.Repositories.Core;
using Kin.KinHub.Core.PostgreSql.Repositories.Recipe;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKinHubCorePostgreSqlInfrastructure(
        this IServiceCollection services,
        Action<Kin.KinHub.Core.PostgreSql.PostgreSqlOptions> configure)
    {
        var options = new Kin.KinHub.Core.PostgreSql.PostgreSqlOptions();
        configure(options);
        options.Validate();

        TypeAdapterConfig.GlobalSettings.NewConfig<Vector, float[]>()
            .MapWith(v => v.ToArray());
        TypeAdapterConfig.GlobalSettings.NewConfig<float[], Vector>()
            .MapWith(f => new Vector(f));

        services.AddDbContext<CoreDbContext>(o =>
            o.UseNpgsql(options.ConnectionString));

        // Core repositories
        services.AddScoped<IFamilyRepository, FamilyRepository>();
        services.AddScoped<IFamilyMemberRepository, FamilyMemberRepository>();
        services.AddScoped<IFamilyRoleRepository, FamilyRoleRepository>();
        services.AddScoped<IMemberRoleRepository, MemberRoleRepository>();
        services.AddScoped<IKinHubServiceRepository, KinHubServiceRepository>();
        services.AddScoped<IFamilyServiceRepository, FamilyServiceRepository>();

        // Recipe repositories (stubs — implement after EF Core Power Tools scaffold)
        services.AddScoped<IRecipeBookRepository, RecipeBookRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
        services.AddScoped<IRecipeStepRepository, RecipeStepRepository>();
        services.AddScoped<IFridgeRepository, FridgeRepository>();
        services.AddScoped<IFridgeIngredientRepository, FridgeIngredientRepository>();

        return services;
    }
}
