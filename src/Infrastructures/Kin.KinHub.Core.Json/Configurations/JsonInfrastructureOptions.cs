namespace Kin.KinHub.Core.Json;

public sealed class JsonInfrastructureOptions
{
    public string DataDirectory { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DataDirectory))
            throw new InvalidOperationException($"{nameof(DataDirectory)} must be configured.");
    }
}
