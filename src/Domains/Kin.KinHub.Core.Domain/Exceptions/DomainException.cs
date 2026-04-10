namespace Kin.KinHub.Core.Domain;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
