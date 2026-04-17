using FluentValidation;

namespace Kin.KinHub.Core.Api.RecipeAssistantFeature;

internal sealed class ParseRecipeRequestValidator : AbstractValidator<ParseRecipeRequest>
{
    public ParseRecipeRequestValidator()
    {
        RuleFor(x => x.RawText).NotEmpty();
    }
}
