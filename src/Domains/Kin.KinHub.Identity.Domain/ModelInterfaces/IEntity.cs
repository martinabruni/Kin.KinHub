namespace Kin.KinHub.Identity.Domain.ModelInterfaces;

public interface IEntity<T>
{
    T Id { get; }
}
