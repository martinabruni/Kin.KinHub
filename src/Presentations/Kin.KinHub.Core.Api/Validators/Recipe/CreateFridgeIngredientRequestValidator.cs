using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators.Recipe;

internal sealed class CreateFridgeIngredientRequestValidator : AbstractValidator<CreateFridgeIngredientRequest>
{
    public CreateFridgeIngredientRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.MeasureUnit)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.FridgeId)
            .NotEmpty();
    }
}
