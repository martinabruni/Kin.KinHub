using Kin.KinHub.Core.Domain;
using System.Text.Json;

namespace Kin.KinHub.Core.Json;

public sealed class FamilyRoleJsonRepository : JsonRepository<FamilyRole, int>, IFamilyRoleRepository
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private static readonly string _seedJson = JsonSerializer.Serialize(
        new List<FamilyRole>
        {
            new() { Id = (int)FamilyRoleType.Admin, Name = "admin", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = (int)FamilyRoleType.Member, Name = "member", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        },
        _jsonOptions);

    public FamilyRoleJsonRepository(string dataDirectory)
        : base(Path.Combine(dataDirectory, "familyRoles.json"))
    {
        EnsureSeeded(Path.Combine(dataDirectory, "familyRoles.json"));
    }

    private static void EnsureSeeded(string filePath)
    {
        var content = File.ReadAllText(filePath);
        if (content.Trim() == "[]")
            File.WriteAllText(filePath, _seedJson);
    }

    /// <inheritdoc/>
    public async Task<FamilyRole?> FindByRoleTypeAsync(
        FamilyRoleType roleType,
        CancellationToken cancellationToken = default)
    {
        var items = await ReadAllAsync();
        return items.FirstOrDefault(r => r.Id == (int)roleType);
    }
}
