namespace Kin.KinHub.Core.Business;

public sealed class CreateFamilyRequest
{
    public required string FamilyName { get; init; }
    public required string OwnerProfileName { get; init; }
    public IReadOnlyList<string> AdditionalMembers { get; init; } = [];
}
