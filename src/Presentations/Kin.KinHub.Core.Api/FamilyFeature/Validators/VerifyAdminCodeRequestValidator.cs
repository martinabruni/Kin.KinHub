using FluentValidation;

namespace Kin.KinHub.Core.Api.FamilyFeature;

internal sealed class VerifyAdminCodeRequestValidator : AbstractValidator<VerifyAdminCodeRequest>
{
    public VerifyAdminCodeRequestValidator()
    {
        RuleFor(x => x.AdminCode)
            .NotEmpty()
            .MaximumLength(100);
    }
}
