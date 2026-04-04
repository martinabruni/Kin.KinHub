using Kin.KinHub.KinHub.Business.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.KinHub.Function;

public sealed class LoginFunction
{
    private readonly IAuthenticationService _authService;

    public LoginFunction(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [Function(nameof(LoginFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var request = await req.ReadFromJsonAsync<LoginRequest>(cancellationToken);

        if (request is null)
            return new BadRequestObjectResult(new { message = "Invalid request body." });

        var result = await _authService.LoginAsync(request, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
