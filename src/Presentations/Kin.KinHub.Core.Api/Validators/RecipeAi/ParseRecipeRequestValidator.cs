using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.RecipeAi;

internal sealed class ParseRecipeRequestValidator : AbstractValidator<ParseRecipeRequest>
{
    public ParseRecipeRequestValidator()
    {
        RuleFor(x => x.RawText).NotEmpty();
    }
}
