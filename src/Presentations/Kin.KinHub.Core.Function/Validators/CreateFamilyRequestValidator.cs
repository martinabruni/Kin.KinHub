using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Function.Validators;

internal sealed class CreateFamilyRequestValidator : AbstractValidator<CreateFamilyRequest>
{
    public CreateFamilyRequestValidator()
    {
        RuleFor(x => x.FamilyName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.OwnerProfileName)
            .NotEmpty()
            .MaximumLength(100);

        RuleForEach(x => x.AdditionalMembers)
            .NotEmpty()
            .MaximumLength(100);
    }
}
