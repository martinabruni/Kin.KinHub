using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators;

internal sealed class VerifyAdminCodeRequestValidator : AbstractValidator<VerifyAdminCodeRequest>
{
    public VerifyAdminCodeRequestValidator()
    {
        RuleFor(x => x.AdminCode)
            .NotEmpty()
            .MaximumLength(100);
    }
}
