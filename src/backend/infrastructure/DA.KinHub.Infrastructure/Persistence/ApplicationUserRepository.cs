using System.Data.Common;
using DA.KinHub.Domain.Common;
using DA.KinHub.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DA.KinHub.Infrastructure.Persistence;

internal sealed class ApplicationUserRepository(KinHubDbContext dbContext) : IApplicationUserRepository
{
    public async Task<ApplicationUser?> FindByExternalIdentityAsync(ExternalIdentity externalIdentity, CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.ApplicationUsers
                .SingleOrDefaultAsync(
                    applicationUser => applicationUser.ExternalIssuer == externalIdentity.Issuer
                        && applicationUser.ExternalObjectId == externalIdentity.ObjectId,
                    cancellationToken);
        }
        catch (Exception exception) when (IsRepositoryUnavailable(exception))
        {
            throw new RepositoryUnavailableException("The application user could not be loaded.", exception);
        }
    }

    public async Task<ApplicationUser> GetOrCreateAsync(ExternalIdentity externalIdentity, DateTimeOffset createdAt, CancellationToken cancellationToken)
    {
        try
        {
            var applicationUser = ApplicationUser.Create(externalIdentity, createdAt);
            var existing = await dbContext.ApplicationUsers
                .SingleOrDefaultAsync(
                    current => current.ExternalIssuer == externalIdentity.Issuer
                        && current.ExternalObjectId == externalIdentity.ObjectId,
                    cancellationToken);

            if (existing is not null)
            {
                return existing;
            }

            dbContext.ApplicationUsers.Add(applicationUser);
            await dbContext.SaveChangesAsync(cancellationToken);
            return applicationUser;
        }
        catch (DbUpdateException exception) when (exception.InnerException is Microsoft.Data.SqlClient.SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627))
        {
            return await dbContext.ApplicationUsers
                .SingleAsync(
                    current => current.ExternalIssuer == externalIdentity.Issuer
                        && current.ExternalObjectId == externalIdentity.ObjectId,
                    cancellationToken);
        }
        catch (Exception exception) when (IsRepositoryUnavailable(exception))
        {
            throw new RepositoryUnavailableException("The application user could not be stored.", exception);
        }
    }

    private static bool IsRepositoryUnavailable(Exception exception) =>
        exception is TimeoutException
        or DbException
        or DbUpdateException { InnerException: DbException };
}
