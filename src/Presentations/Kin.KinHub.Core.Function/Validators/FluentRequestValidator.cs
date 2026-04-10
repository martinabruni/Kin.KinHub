using FluentValidation;
using Kin.KinHub.Core.Function.Validators.Interfaces;
using Kin.KinHub.Core.Function.Validators.Models;

namespace Kin.KinHub.Core.Function.Validators;

internal sealed class FluentRequestValidator<T> : IRequestValidator<T>
{
    private readonly IValidator<T> _validator;

    public FluentRequestValidator(IValidator<T> validator)
    {
        _validator = validator;
    }

    /// <inheritdoc/>
    public async Task<RequestValidationResult> ValidateAsync(T request, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);

        if (result.IsValid)
            return RequestValidationResult.Success();

        var errors = result.Errors
            .Select(e => e.ErrorMessage)
            .ToList();

        return RequestValidationResult.Failure(errors);
    }
}
