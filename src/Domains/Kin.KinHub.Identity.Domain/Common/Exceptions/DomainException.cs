namespace Kin.KinHub.Identity.Domain.Common;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
