using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Jwt;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Kin.KinHub.Identity.Function.Middlewares;

public sealed class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly ITokenValidator _tokenValidator;

    public JwtAuthenticationMiddleware(ITokenValidator tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }

    public async Task Invoke(
        FunctionContext functionContext,
        FunctionExecutionDelegate next)
    {
        var requestData = await functionContext.GetHttpRequestDataAsync();
        var authHeader = requestData?.Headers.TryGetValues("Authorization", out var values) is true
            ? values.FirstOrDefault()
            : null;

        if (!string.IsNullOrWhiteSpace(authHeader)
            && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            var claims = _tokenValidator.ValidateAccessToken(token);

            if (claims is not null)
            {
                var currentUser = functionContext.InstanceServices.GetRequiredService<CurrentUser>();
                currentUser.Populate(claims);
            }
        }

        await next(functionContext);
    }
}
