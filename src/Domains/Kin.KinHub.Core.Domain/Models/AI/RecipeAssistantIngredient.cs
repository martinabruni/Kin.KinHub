namespace Kin.KinHub.Core.Domain.AI;

public sealed record RecipeAssistantIngredient
{
    public required string Name { get; init; }
    public required decimal Quantity { get; init; }
    public required string Unit { get; init; }
}
