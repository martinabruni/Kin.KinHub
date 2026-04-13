using FluentValidation;
using Kin.KinHub.Core.Business;

namespace Kin.KinHub.Core.Api.Validators;

internal sealed class ToggleFamilyServiceRequestValidator : AbstractValidator<ToggleFamilyServiceRequest>
{
    public ToggleFamilyServiceRequestValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0);
    }
}
