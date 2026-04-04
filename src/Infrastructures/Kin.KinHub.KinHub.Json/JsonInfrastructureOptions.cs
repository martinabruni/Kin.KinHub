namespace Kin.KinHub.KinHub.Json;

public sealed class JsonInfrastructureOptions
{
    public string DataDirectory { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DataDirectory))
            throw new InvalidOperationException($"{nameof(DataDirectory)} must be configured.");
    }
}
