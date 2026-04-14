using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain.Recipes;

public sealed class Recipe : BaseDeletableEntity<Guid>
{
    public required string Name { get; set; }
    public string? Backstory { get; set; }
    public required TimeSpan FinalTime { get; set; }
    public required int Portions { get; set; }
    public required Guid RecipeBookId { get; set; }
}
