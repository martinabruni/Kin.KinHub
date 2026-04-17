namespace Kin.KinHub.Core.Business.RecipeAssistantFeature;

public sealed class SuggestRecipesRequest
{
    public required Guid FridgeId { get; init; }
}
