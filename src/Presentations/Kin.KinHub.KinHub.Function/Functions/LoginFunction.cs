using Kin.KinHub.KinHub.Business.Auth;
using Kin.KinHub.KinHub.Function.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace Kin.KinHub.KinHub.Function;

public sealed class LoginFunction
{
    private readonly IAuthenticationService _authService;
    private readonly IRequestValidator<LoginRequest> _validator;

    public LoginFunction(
        IAuthenticationService authService,
        IRequestValidator<LoginRequest> validator)
    {
        _authService = authService;
        _validator = validator;
    }

    [Function(nameof(LoginFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = await req.ReadFromJsonAsync<LoginRequest>(cancellationToken);

            if (request is null)
                return new BadRequestObjectResult(new { message = "Invalid request body." });

            var validation = await _validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
                return new BadRequestObjectResult(new { errors = validation.Errors });

            var result = await _authService.LoginAsync(request, cancellationToken);

            return HttpResultMapper.ToActionResult(result);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { errors = new[] { "Malformed JSON in request body." } });
        }
    }
}
