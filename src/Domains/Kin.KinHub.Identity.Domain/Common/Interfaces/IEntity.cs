namespace Kin.KinHub.Identity.Domain.Common;

public interface IEntity<T>
{
    T Id { get; }
}
