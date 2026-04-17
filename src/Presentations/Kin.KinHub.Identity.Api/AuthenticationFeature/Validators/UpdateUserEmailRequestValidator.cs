using FluentValidation;

namespace Kin.KinHub.Identity.Api.AuthenticationFeature;

internal sealed class UpdateUserEmailRequestValidator : AbstractValidator<UpdateUserEmailRequest>
{
    public UpdateUserEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();
    }
}
