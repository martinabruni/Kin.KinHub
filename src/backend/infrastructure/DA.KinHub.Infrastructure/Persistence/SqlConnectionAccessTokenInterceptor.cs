using System.Data.Common;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DA.KinHub.Infrastructure.Persistence;

internal sealed class SqlConnectionAccessTokenInterceptor(DatabaseOptions options, TokenCredential credential) : DbConnectionInterceptor
{
    private const string ManagedIdentityMode = "ManagedIdentity";
    private static readonly TokenRequestContext SqlTokenRequestContext = new(["https://database.windows.net/.default"]);

    public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
    {
        ApplyAccessToken(connection, CancellationToken.None).GetAwaiter().GetResult();
        return result;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        await ApplyAccessToken(connection, cancellationToken);
        return result;
    }

    private async Task ApplyAccessToken(DbConnection connection, CancellationToken cancellationToken)
    {
        if (!string.Equals(options.Mode, ManagedIdentityMode, StringComparison.Ordinal) || connection is not SqlConnection sqlConnection)
        {
            return;
        }

        var accessToken = await credential.GetTokenAsync(SqlTokenRequestContext, cancellationToken);
        sqlConnection.AccessToken = accessToken.Token;
    }
}
