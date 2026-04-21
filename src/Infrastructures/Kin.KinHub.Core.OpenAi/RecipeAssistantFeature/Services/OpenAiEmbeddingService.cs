using Azure;
using Azure.AI.OpenAI;
using Kin.KinHub.Core.OpenAi.Common;
using OpenAI.Embeddings;

namespace Kin.KinHub.Core.OpenAi.RecipeAssistantFeature;

internal sealed class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;

    public OpenAiEmbeddingService(OpenAiOptions options)
    {
        var client = new AzureOpenAIClient(new Uri(options.Endpoint), new AzureKeyCredential(options.ApiKey));
        _embeddingClient = client.GetEmbeddingClient(options.EmbeddingDeploymentName);
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await _embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
        return result.Value.ToFloats().ToArray();
    }
}
