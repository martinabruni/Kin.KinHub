using FluentValidation;
using Kin.KinHub.KinHub.Business.Auth;

namespace Kin.KinHub.KinHub.Function.Validators.Auth;

internal sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
