using Kin.KinHub.Core.Business.Common;

namespace Kin.KinHub.Core.Business;

/// <summary>
/// Business service for family management operations.
/// </summary>
public interface IFamilyService
{
    /// <summary>
    /// Creates a new family for the authenticated user with an initial owner profile and optional additional members.
    /// </summary>
    Task<Result<CreateFamilyResponse>> CreateFamilyAsync(
        CreateFamilyRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new member profile to an existing family owned by the given user.
    /// </summary>
    Task<Result<AddFamilyMemberResponse>> AddFamilyMemberAsync(
        Guid familyId,
        AddFamilyMemberRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the family and its members for the given user.
    /// </summary>
    Task<Result<FamilyDetailResponse>> GetFamilyAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
