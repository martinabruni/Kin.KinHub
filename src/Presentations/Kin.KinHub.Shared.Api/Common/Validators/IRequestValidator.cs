
namespace Kin.KinHub.Shared.Api.Common;

public interface IRequestValidator<T>
{
    Task<RequestValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default);
}
