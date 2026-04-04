namespace Kin.KinHub.KinHub.Domain;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message) { }
}
