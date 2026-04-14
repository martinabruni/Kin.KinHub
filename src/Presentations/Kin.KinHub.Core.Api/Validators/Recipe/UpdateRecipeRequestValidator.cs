using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.Recipe;

internal sealed class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Backstory)
            .MaximumLength(2000)
            .When(x => x.Backstory is not null);

        RuleFor(x => x.FinalTime)
            .GreaterThan(TimeSpan.Zero);

        RuleFor(x => x.Portions)
            .GreaterThan(0);
    }
}
