using Kin.KinHub.Core.Domain;
using System.Text.Json;

namespace Kin.KinHub.Core.Json;

public sealed class KinHubServiceJsonRepository : JsonRepository<KinHubService, int>, IKinHubServiceRepository
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private static readonly string _seedJson = JsonSerializer.Serialize(
        new List<KinHubService>
        {
            new()
            {
                Id = (int)KinHubServiceType.KinConsole,
                Name = "KinConsole",
                BaseUrl = "/kin-console",
                IsActive = true,
                IsAdminOnly = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = (int)KinHubServiceType.KinRecipe,
                Name = "KinRecipe",
                BaseUrl = "/kin-recipe",
                IsActive = false,
                IsAdminOnly = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        },
        _jsonOptions);

    public KinHubServiceJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "kinHubServices.json"))
    {
        EnsureSeeded(Path.Combine(dataDirectory, "kinHubServices.json"));
    }

    private static void EnsureSeeded(string filePath)
    {
        var content = File.ReadAllText(filePath);
        if (content.Trim() == "[]")
            File.WriteAllText(filePath, _seedJson);
    }

    /// <inheritdoc/>
    public async Task<KinHubService?> FindByServiceTypeAsync(
        KinHubServiceType serviceType,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(s => s.Id == (int)serviceType);
    }
}
