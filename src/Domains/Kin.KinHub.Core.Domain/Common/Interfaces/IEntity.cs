namespace Kin.KinHub.Core.Domain.Common;

/// <summary>
/// Represents a domain entity with a typed identifier.
/// </summary>
/// <typeparam name="T">The type of the entity identifier.</typeparam>
public interface IEntity<T>
{
    T Id { get; }
}
