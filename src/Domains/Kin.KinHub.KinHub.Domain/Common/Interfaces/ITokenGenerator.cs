using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Domain.Common;

/// <summary>
/// Generates access and refresh tokens for authenticated users.
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generates a signed JWT access token containing standard claims (sub, email, roles, iss, exp).
    /// </summary>
    string GenerateAccessToken(IdentityUser user);

    /// <summary>
    /// Generates a cryptographically-random opaque refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Returns the configured access-token lifetime in seconds.
    /// </summary>
    int AccessTokenExpirySeconds { get; }
}
