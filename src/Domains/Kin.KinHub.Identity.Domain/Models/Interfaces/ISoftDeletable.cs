namespace Kin.KinHub.Identity.Domain.Models.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
