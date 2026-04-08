using FluentValidation;
using Kin.KinHub.KinHub.Business.Auth;

namespace Kin.KinHub.KinHub.Function.Validators.Auth;

internal sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
