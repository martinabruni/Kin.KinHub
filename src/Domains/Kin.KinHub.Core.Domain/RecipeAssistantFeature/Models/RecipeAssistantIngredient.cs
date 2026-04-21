namespace Kin.KinHub.Core.Domain.RecipeAssistantFeature;

public sealed record RecipeAssistantIngredient
{
    public required string Name { get; init; }
    public required decimal Quantity { get; init; }
    public required string Unit { get; init; }
}
