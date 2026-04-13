using FluentValidation;
using Kin.KinHub.Identity.Business.Models;

namespace Kin.KinHub.Identity.Api.Validators;

internal sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
