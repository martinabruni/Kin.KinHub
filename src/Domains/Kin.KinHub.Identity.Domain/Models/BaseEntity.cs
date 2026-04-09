using Kin.KinHub.Identity.Domain.ModelInterfaces;

namespace Kin.KinHub.Identity.Domain.Models;

public abstract class BaseEntity<T> : IEntity<T>, IAuditable
{
    public required T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
