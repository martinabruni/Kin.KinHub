namespace Kin.KinHub.Identity.Domain.Models.Interfaces;

/// <summary>
/// Represents the authenticated user context for the current request.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the unique identifier of the authenticated user.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Gets the email address of the authenticated user.
    /// </summary>
    string Email { get; }

    /// <summary>
    /// Gets the roles assigned to the authenticated user.
    /// </summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Gets a value indicating whether the current request is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
