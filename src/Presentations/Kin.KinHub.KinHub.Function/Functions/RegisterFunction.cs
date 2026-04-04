using Kin.KinHub.KinHub.Business.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.KinHub.Function;

public sealed class RegisterFunction
{
    private readonly IAuthenticationService _authService;

    public RegisterFunction(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [Function(nameof(RegisterFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var request = await req.ReadFromJsonAsync<RegisterRequest>(cancellationToken);

        if (request is null)
            return new BadRequestObjectResult(new { message = "Invalid request body." });

        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result.IsSuccess)
            return new ObjectResult(result.Value) { StatusCode = StatusCodes.Status201Created };

        return HttpResultMapper.ToActionResult(result);
    }
}
