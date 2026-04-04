using Kin.KinHub.KinHub.Business.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.KinHub.Function;

public sealed class LogoutFunction
{
    private readonly IAuthenticationService _authService;

    public LogoutFunction(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [Function(nameof(LogoutFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/logout/{sessionId:guid}")] HttpRequest req,
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LogoutAsync(sessionId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
