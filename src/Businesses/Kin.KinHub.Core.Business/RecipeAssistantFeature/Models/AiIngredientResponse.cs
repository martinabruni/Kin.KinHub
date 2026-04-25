namespace Kin.KinHub.Core.Business.RecipeAssistantFeature;

public sealed class AiIngredientResponse
{
    public required string Name { get; init; }
    public required decimal Quantity { get; init; }
    public required string MeasureUnit { get; init; }
}
