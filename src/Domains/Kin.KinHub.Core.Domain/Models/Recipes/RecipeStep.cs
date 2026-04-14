using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain.Recipes;

public sealed class RecipeStep : BaseDeletableEntity<Guid>
{
    public required int Order { get; set; }
    public required string Description { get; set; }
    public required Guid RecipeId { get; set; }
}
