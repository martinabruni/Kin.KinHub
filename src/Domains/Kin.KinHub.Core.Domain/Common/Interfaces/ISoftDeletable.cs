namespace Kin.KinHub.Core.Domain.Common;

/// <summary>
/// Represents an entity that supports soft deletion.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
