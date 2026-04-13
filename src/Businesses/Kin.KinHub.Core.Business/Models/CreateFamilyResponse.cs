namespace Kin.KinHub.Core.Business;

public sealed class CreateFamilyResponse
{
    public required Guid FamilyId { get; init; }
    public required Guid AdminMemberId { get; init; }
}
