using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators;

internal sealed class UpdateFamilyMemberRequestValidator : AbstractValidator<UpdateFamilyMemberRequest>
{
    public UpdateFamilyMemberRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
