using Kin.KinHub.Core.Domain.Common;

namespace Kin.KinHub.Core.Domain.Recipes;

public sealed class FridgeIngredient : BaseEmbeddingEntity<Guid>
{
    public required string Name { get; set; }
    public required string MeasureUnit { get; set; }
    public required decimal Quantity { get; set; }
    public required Guid FridgeId { get; set; }
}
