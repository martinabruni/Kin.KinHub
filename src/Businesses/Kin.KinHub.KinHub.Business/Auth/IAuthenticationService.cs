using Kin.KinHub.KinHub.Business.Common;

namespace Kin.KinHub.KinHub.Business.Auth;

public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user with the KinHub provider (email + password).
    /// </summary>
    Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user with email and password, returning a new session.
    /// </summary>
    Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates an active session.
    /// </summary>
    Task<Result<bool>> LogoutAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

    // TODO: implement email verification
    // Task<Result<bool>> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);

    // TODO: implement password reset
    // Task<Result<bool>> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
    // Task<Result<bool>> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}
