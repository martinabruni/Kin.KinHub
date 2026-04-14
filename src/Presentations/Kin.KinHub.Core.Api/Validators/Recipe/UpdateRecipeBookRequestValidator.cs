using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.Recipe;

internal sealed class UpdateRecipeBookRequestValidator : AbstractValidator<UpdateRecipeBookRequest>
{
    public UpdateRecipeBookRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}
