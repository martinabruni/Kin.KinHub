namespace Kin.KinHub.Identity.Domain.Common;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message) { }
}
