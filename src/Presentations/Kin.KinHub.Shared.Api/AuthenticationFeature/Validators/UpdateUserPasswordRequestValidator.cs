using FluentValidation;

namespace Kin.KinHub.Shared.Api.AuthenticationFeature;

internal sealed class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
{
    public UpdateUserPasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8);
    }
}
