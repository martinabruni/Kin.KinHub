namespace Kin.KinHub.Core.Business;

public sealed class UpdateRecipeIngredientRequest
{
    public required string Name { get; init; }
    public required string MeasureUnit { get; init; }
    public required decimal Quantity { get; init; }
}
