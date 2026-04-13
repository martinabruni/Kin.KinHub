namespace Kin.KinHub.Identity.Domain.Models.Interfaces;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
