using FluentValidation;

namespace Kin.KinHub.Shared.Api.FamilyFeature;

internal sealed class UpdateAdminCodeRequestValidator : AbstractValidator<UpdateAdminCodeRequest>
{
    public UpdateAdminCodeRequestValidator()
    {
        RuleFor(x => x.CurrentCode)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.NewCode)
            .NotEmpty()
            .MaximumLength(100);
    }
}
