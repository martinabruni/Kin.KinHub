using FluentValidation;

namespace Kin.KinHub.Core.Api.FamilyFeature;

internal sealed class UpdateFamilyMemberRequestValidator : AbstractValidator<UpdateFamilyMemberRequest>
{
    public UpdateFamilyMemberRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
