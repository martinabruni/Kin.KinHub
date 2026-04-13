namespace Kin.KinHub.Identity.Domain.Exceptions;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message) { }
}
