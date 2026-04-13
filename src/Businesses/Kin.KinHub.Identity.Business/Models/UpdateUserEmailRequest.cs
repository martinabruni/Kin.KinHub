namespace Kin.KinHub.Identity.Business.Models;

public sealed class UpdateUserEmailRequest
{
    public required string NewEmail { get; init; }
}
