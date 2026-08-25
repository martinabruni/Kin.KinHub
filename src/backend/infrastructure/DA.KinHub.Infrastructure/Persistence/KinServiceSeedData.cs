namespace DA.KinHub.Infrastructure.Persistence;

internal static class KinServiceSeedData
{
    public static readonly Guid KinListServiceId = Guid.Parse("a5f1cb74-e8f7-4cdc-8d95-f1ad39090d18");
    public static readonly Guid KinListLocalizationItId = Guid.Parse("fc4db75e-7813-4ee7-92b5-2ce17fd90518");
    public static readonly Guid KinListLocalizationEnId = Guid.Parse("8ec4ca56-9097-4d4d-8c88-cc9224d1e0d0");
    public static readonly DateTimeOffset SeedTimestamp = DateTimeOffset.Parse("2026-07-30T12:00:00Z", System.Globalization.CultureInfo.InvariantCulture);
}
