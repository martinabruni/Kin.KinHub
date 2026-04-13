using Kin.KinHub.Identity.Api.Validators.Models;

namespace Kin.KinHub.Identity.Api.Validators.Interfaces;

public interface IRequestValidator<T>
{
    Task<RequestValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default);
}
