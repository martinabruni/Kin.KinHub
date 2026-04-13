namespace Kin.KinHub.Core.Domain;

public sealed class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message) { }
}
