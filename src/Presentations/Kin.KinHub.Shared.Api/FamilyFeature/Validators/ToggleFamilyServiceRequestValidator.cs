using FluentValidation;

namespace Kin.KinHub.Shared.Api.FamilyFeature;

internal sealed class ToggleFamilyServiceRequestValidator : AbstractValidator<ToggleFamilyServiceRequest>
{
    public ToggleFamilyServiceRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0);
    }
}
