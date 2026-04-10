using Kin.KinHub.Core.Function.Validators.Models;

namespace Kin.KinHub.Core.Function.Validators.Interfaces;

/// <summary>
/// Validates a request of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The request model type to validate.</typeparam>
public interface IRequestValidator<T>
{
    /// <summary>
    /// Validates the given request asynchronously.
    /// </summary>
    Task<RequestValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default);
}
