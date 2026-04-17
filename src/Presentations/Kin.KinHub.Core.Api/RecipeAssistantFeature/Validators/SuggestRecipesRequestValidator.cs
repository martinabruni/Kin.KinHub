using FluentValidation;

namespace Kin.KinHub.Core.Api.RecipeAssistantFeature;

internal sealed class SuggestRecipesRequestValidator : AbstractValidator<SuggestRecipesRequest>
{
    public SuggestRecipesRequestValidator()
    {
        RuleFor(x => x.FridgeId).NotEmpty();
    }
}
