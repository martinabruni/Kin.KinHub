namespace Kin.KinHub.Identity.Domain.Exceptions;

public sealed class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityName, string field, object value)
        : base($"{entityName} with {field} '{value}' already exists.") { }
}
