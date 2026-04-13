using Kin.KinHub.Identity.Api.Validators.Interfaces;
using Kin.KinHub.Identity.Business.Interfaces;
using Kin.KinHub.Identity.Business.Models;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly IRequestValidator<LoginRequest> _loginValidator;
    private readonly IRequestValidator<RegisterRequest> _registerValidator;
    private readonly IRequestValidator<RefreshRequest> _refreshValidator;
    private readonly ICurrentUser _currentUser;

    public AuthController(
        IAuthenticationService authService,
        IRequestValidator<LoginRequest> loginValidator,
        IRequestValidator<RegisterRequest> registerValidator,
        IRequestValidator<RefreshRequest> refreshValidator,
        ICurrentUser currentUser)
    {
        _authService = authService;
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _refreshValidator = refreshValidator;
        _currentUser = currentUser;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _loginValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _authService.LoginAsync(request, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _registerValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result.IsSuccess)
            return new ObjectResult(result.Value) { StatusCode = StatusCodes.Status201Created };

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(
        [FromBody] RefreshRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _refreshValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> MeAsync(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _authService.GetCurrentUserAsync(_currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(
        [FromBody] RefreshRequest? request,
        CancellationToken cancellationToken)
    {
        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _refreshValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
