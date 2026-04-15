using Kin.KinHub.Core.Domain.Interfaces.AI;
using Kin.KinHub.OpenAi.Common;
using Kin.KinHub.OpenAi.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKinHubOpenAiInfrastructure(
        this IServiceCollection services,
        Action<OpenAiOptions> configure)
    {
        var options = new OpenAiOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddScoped<IEmbeddingService, OpenAiEmbeddingService>();
        services.AddScoped<IRecipeMissingIngredientsService, OpenAiRecipeMissingIngredientsService>();
        services.AddScoped<IRecipeAssistantService, OpenAiRecipeAssistantService>();

        return services;
    }
}
