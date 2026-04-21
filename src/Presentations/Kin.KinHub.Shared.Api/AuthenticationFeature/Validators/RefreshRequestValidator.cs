using FluentValidation;

namespace Kin.KinHub.Shared.Api.AuthenticationFeature;

internal sealed class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
