using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Function.Validators;

internal sealed class AddFamilyMemberRequestValidator : AbstractValidator<AddFamilyMemberRequest>
{
    public AddFamilyMemberRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
