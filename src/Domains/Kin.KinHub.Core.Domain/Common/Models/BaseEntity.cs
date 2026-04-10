namespace Kin.KinHub.Core.Domain.Common;

public abstract class BaseEntity<T> : IEntity<T>, IAuditable
{
    public required T Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
