namespace Kin.KinHub.Core.Business;

public sealed class RecipeIngredientResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string MeasureUnit { get; init; }
    public required decimal Quantity { get; init; }
    public required Guid RecipeId { get; init; }
}
