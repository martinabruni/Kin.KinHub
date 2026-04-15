using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.RecipeAi;

internal sealed class AdaptRecipeRequestValidator : AbstractValidator<AdaptRecipeRequest>
{
    public AdaptRecipeRequestValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.Constraints).NotEmpty();
    }
}
