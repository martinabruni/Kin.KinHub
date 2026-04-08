using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Jwt;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.DependencyInjection;

namespace Kin.KinHub.KinHub.Function.Middleware;

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
