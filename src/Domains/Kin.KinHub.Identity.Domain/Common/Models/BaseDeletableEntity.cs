
namespace Kin.KinHub.Identity.Domain.Common;

public abstract class BaseDeletableEntity<T> : BaseEntity<T>, ISoftDeletable
{
    public bool IsDeleted { get; set; }
}