namespace Kin.KinHub.Identity.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
