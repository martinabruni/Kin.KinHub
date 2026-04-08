namespace Kin.KinHub.KinHub.Domain.Common;

/// <summary>
/// Validates JWT access tokens and extracts claims.
/// </summary>
public interface ITokenValidator
{
    /// <summary>
    /// Validates the given JWT and returns the extracted claims, or null if the token is invalid.
    /// </summary>
    TokenClaims? ValidateAccessToken(string token);
}

/// <summary>
/// Claims extracted from a validated JWT access token.
/// </summary>
public sealed record TokenClaims(
    Guid UserId,
    string Email,
    IReadOnlyList<string> Roles);
