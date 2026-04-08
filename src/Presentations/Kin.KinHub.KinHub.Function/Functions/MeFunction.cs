using Kin.KinHub.KinHub.Business.Auth;
using Kin.KinHub.KinHub.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Kin.KinHub.KinHub.Function;

public sealed class MeFunction
{
    private readonly IAuthenticationService _authService;
    private readonly ITokenValidator _tokenValidator;

    public MeFunction(
        IAuthenticationService authService,
        ITokenValidator tokenValidator)
    {
        _authService = authService;
        _tokenValidator = tokenValidator;
    }

    [Function(nameof(MeFunction))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/me")] HttpRequest req,
        CancellationToken cancellationToken)
    {
        var authHeader = req.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return new UnauthorizedObjectResult(new { message = "Missing or invalid Authorization header." });

        var token = authHeader["Bearer ".Length..].Trim();
        var claims = _tokenValidator.ValidateAccessToken(token);

        if (claims is null)
            return new UnauthorizedObjectResult(new { message = "Invalid or expired token." });

        var result = await _authService.GetCurrentUserAsync(claims.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
