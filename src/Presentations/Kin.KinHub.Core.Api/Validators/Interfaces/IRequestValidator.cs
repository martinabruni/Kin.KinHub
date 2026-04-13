using Kin.KinHub.Core.Api.Validators.Models;

namespace Kin.KinHub.Core.Api.Validators.Interfaces;

public interface IRequestValidator<T>
{
    Task<RequestValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default);
}
