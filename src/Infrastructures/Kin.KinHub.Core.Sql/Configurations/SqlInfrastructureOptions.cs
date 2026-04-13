namespace Kin.KinHub.Core.Sql;

public sealed class SqlInfrastructureOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
            throw new InvalidOperationException($"{nameof(ConnectionString)} must be configured.");
    }
}
