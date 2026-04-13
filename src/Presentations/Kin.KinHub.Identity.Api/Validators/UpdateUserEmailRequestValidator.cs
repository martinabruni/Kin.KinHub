using FluentValidation;
using Kin.KinHub.Identity.Business.Models;

namespace Kin.KinHub.Identity.Api.Validators;

internal sealed class UpdateUserEmailRequestValidator : AbstractValidator<UpdateUserEmailRequest>
{
    public UpdateUserEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();
    }
}
