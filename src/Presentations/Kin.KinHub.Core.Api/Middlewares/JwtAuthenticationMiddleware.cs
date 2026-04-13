using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Jwt;

namespace Kin.KinHub.Core.Api.Middlewares;

public sealed class JwtAuthenticationMiddleware : IMiddleware
{
    private readonly ITokenValidator _tokenValidator;

    public JwtAuthenticationMiddleware(ITokenValidator tokenValidator)
    {
        _tokenValidator = tokenValidator;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authHeader)
            && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader["Bearer ".Length..].Trim();
            var claims = _tokenValidator.ValidateAccessToken(token);

            if (claims is not null)
            {
                var currentUser = context.RequestServices.GetRequiredService<CurrentUser>();
                currentUser.Populate(claims);
            }
        }

        await next(context);
    }
}
