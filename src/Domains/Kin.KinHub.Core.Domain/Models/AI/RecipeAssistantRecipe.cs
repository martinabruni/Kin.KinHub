namespace Kin.KinHub.Core.Domain.AI;

public sealed record RecipeAssistantRecipe
{
    public required string Name { get; init; }
    public string? Backstory { get; init; }
    public required TimeSpan FinalTime { get; init; }
    public required int Portions { get; init; }
    public required IReadOnlyList<RecipeAssistantIngredient> Ingredients { get; init; }
    public required IReadOnlyList<RecipeAssistantStep> Steps { get; init; }
}
