namespace Kin.KinHub.Core.Domain.RecipeAssistantFeature;

public sealed record RecipeAssistantStep
{
    public required int Order { get; init; }
    public required string Description { get; init; }
}
