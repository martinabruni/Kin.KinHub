
namespace Kin.KinHub.Identity.Domain.Common;

public abstract class BaseActivableEntity<T> : BaseEntity<T>, IActivable
{
    public bool IsActive { get; set; }
}
