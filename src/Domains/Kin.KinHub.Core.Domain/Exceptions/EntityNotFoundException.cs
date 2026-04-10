namespace Kin.KinHub.Core.Domain;

public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.") { }
}
