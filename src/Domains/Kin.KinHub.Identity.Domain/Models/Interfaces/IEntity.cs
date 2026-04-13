namespace Kin.KinHub.Identity.Domain.Models.Interfaces;

public interface IEntity<T>
{
    T Id { get; }
}
