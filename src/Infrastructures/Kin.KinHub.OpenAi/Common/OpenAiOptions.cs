namespace Kin.KinHub.OpenAi.Common;

public sealed class OpenAiOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string EmbeddingDeploymentName { get; set; } = "text-embedding-3-small";
    public string ChatDeploymentName { get; set; } = "gpt-4o";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Endpoint))
            throw new InvalidOperationException($"{nameof(OpenAiOptions)}.{nameof(Endpoint)} is required.");
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new InvalidOperationException($"{nameof(OpenAiOptions)}.{nameof(ApiKey)} is required.");
    }
}
