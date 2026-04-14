using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Domain.Interfaces;

/// <summary>
/// Generates access and refresh tokens for authenticated users.
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generates a signed JWT access token containing standard claims (sub, email, roles, iss, exp).
    /// </summary>
    string GenerateAccessToken(KinUser user, IReadOnlyList<string> roles);

    /// <summary>
    /// Generates a cryptographically-random opaque refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Returns the configured access-token lifetime in seconds.
    /// </summary>
    int AccessTokenExpirySeconds { get; }
}
