namespace Kin.KinHub.Identity.Domain.ModelInterfaces;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
