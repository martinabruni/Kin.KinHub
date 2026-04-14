using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.Recipe;

internal sealed class UpdateRecipeStepRequestValidator : AbstractValidator<UpdateRecipeStepRequest>
{
    public UpdateRecipeStepRequestValidator()
    {
        RuleFor(x => x.Order)
            .GreaterThan(0);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
