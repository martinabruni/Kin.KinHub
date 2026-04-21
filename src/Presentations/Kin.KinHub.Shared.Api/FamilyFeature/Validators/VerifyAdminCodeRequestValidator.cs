using FluentValidation;

namespace Kin.KinHub.Shared.Api.FamilyFeature;

internal sealed class VerifyAdminCodeRequestValidator : AbstractValidator<VerifyAdminCodeRequest>
{
    public VerifyAdminCodeRequestValidator()
    {
        RuleFor(x => x.AdminCode)
            .NotEmpty()
            .MaximumLength(100);
    }
}
