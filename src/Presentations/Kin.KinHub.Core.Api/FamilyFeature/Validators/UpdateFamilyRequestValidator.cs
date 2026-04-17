using FluentValidation;

namespace Kin.KinHub.Core.Api.FamilyFeature;

internal sealed class UpdateFamilyRequestValidator : AbstractValidator<UpdateFamilyRequest>
{
    public UpdateFamilyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);
    }
}
