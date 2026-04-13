using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators;

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
