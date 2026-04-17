namespace Kin.KinHub.Identity.Business.AuthenticationFeature;

public sealed class UpdateUserEmailRequest
{
    public required string NewEmail { get; init; }
}
