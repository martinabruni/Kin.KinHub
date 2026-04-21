namespace Kin.KinHub.Identity.Domain.Common;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
