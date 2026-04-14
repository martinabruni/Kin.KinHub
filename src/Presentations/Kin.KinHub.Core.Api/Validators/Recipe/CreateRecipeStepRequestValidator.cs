using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.Recipe;

internal sealed class CreateRecipeStepRequestValidator : AbstractValidator<CreateRecipeStepRequest>
{
    public CreateRecipeStepRequestValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.RecipeId)
            .NotEmpty();
    }
}
