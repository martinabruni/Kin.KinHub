namespace Kin.KinHub.KinHub.Domain.Common;

public abstract class BaseDeletableEntity<T> : BaseEntity<T>, ISoftDeletable
{
    public bool IsDeleted { get; set; }
}

public abstract class BaseActivableEntity<T> : BaseEntity<T>, IActivable
{
    public bool IsActive { get; set; }
}