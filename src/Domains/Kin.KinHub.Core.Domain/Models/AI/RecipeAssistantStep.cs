namespace Kin.KinHub.Core.Domain.AI;

public sealed record RecipeAssistantStep
{
    public required int Order { get; init; }
    public required string Description { get; init; }
}
