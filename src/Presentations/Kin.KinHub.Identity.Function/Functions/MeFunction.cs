using Kin.KinHub.Identity.Business.Interfaces;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.Identity.Function;

public sealed class MeFunction
{
    private readonly IAuthenticationService _authService;
    private readonly ICurrentUser _currentUser;

    public MeFunction(
        IAuthenticationService authService,
        ICurrentUser currentUser)
    {
        _authService = authService;
        _currentUser = currentUser;
    }

    [Function(nameof(MeFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/me")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return new UnauthorizedObjectResult(new { message = "Missing or invalid Authorization header." });

        var result = await _authService.GetCurrentUserAsync(_currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
