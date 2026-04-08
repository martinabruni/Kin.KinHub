using Kin.KinHub.KinHub.Business.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.KinHub.Function;

public sealed class RefreshFunction
{
    private readonly IAuthenticationService _authService;

    public RefreshFunction(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [Function(nameof(RefreshFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/refresh")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var request = await req.ReadFromJsonAsync<RefreshRequest>(cancellationToken);

        if (request is null)
            return new BadRequestObjectResult(new { message = "Invalid request body." });

        var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
