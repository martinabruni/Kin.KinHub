using Kin.KinHub.Identity.Business.Enums;
using Kin.KinHub.Identity.Business.Models;

namespace Kin.KinHub.Identity.Business.Interfaces;

public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user with the KinHub provider (email + password).
    /// </summary>
    Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user with email and password, returning JWT access and refresh tokens.
    /// </summary>
    Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exchanges a valid refresh token for a new access/refresh token pair.
    /// </summary>
    Task<Result<LoginResponse>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes the given refresh token (logout).
    /// </summary>
    Task<Result<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the profile of the authenticated user.
    /// </summary>
    Task<Result<UserProfileResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
