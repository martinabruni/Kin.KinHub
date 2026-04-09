using Kin.KinHub.Identity.Business.Interfaces;
using Kin.KinHub.Identity.Business.Models;
using Kin.KinHub.Identity.Function.Validators.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace Kin.KinHub.Identity.Function;

public sealed class RefreshFunction
{
    private readonly IAuthenticationService _authService;
    private readonly IRequestValidator<RefreshRequest> _validator;

    public RefreshFunction(
        IAuthenticationService authService,
        IRequestValidator<RefreshRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    [Function(nameof(RefreshFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/refresh")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = await req.ReadFromJsonAsync<RefreshRequest>(cancellationToken);

            if (request is null)
                return new BadRequestObjectResult(new { message = "Invalid request body." });

            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
                return new BadRequestObjectResult(new { errors = validation.Errors });

            var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

            return HttpResultMapper.ToActionResult(result);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { errors = new[] { "Malformed JSON in request body." } });
        }
    }
}
