using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.RecipeAi;

internal sealed class SuggestRecipesRequestValidator : AbstractValidator<SuggestRecipesRequest>
{
    public SuggestRecipesRequestValidator()
    {
        RuleFor(x => x.FridgeId).NotEmpty();
    }
}
