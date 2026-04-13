using Kin.KinHub.Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kin.KinHub.Identity.Sql;

/// <inheritdoc/>
public sealed class IdentityPasswordHasher : IPasswordHasher
{
    private static readonly PasswordHasher<object> _inner = new();
    private static readonly object _dummy = new();

    /// <inheritdoc/>
    public string Hash(string plainPassword) =>
        _inner.HashPassword(_dummy, plainPassword);

    /// <inheritdoc/>
    public bool Verify(string plainPassword, string hashedPassword) =>
        _inner.VerifyHashedPassword(_dummy, hashedPassword, plainPassword)
            is not PasswordVerificationResult.Failed;
}
