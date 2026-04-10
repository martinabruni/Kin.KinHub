namespace Kin.KinHub.Identity.Domain.Models;

public sealed class RefreshToken : BaseEntity<Guid>
{
    public required Guid UserId { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }
    public bool Revoked { get; set; }
}
