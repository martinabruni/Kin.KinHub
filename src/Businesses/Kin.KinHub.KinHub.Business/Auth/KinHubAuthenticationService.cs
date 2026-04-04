using Kin.KinHub.KinHub.Business.Common;
using Kin.KinHub.KinHub.Domain;
using Kin.KinHub.KinHub.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using AspNetIdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;
using DomainIdentityUser = Kin.KinHub.KinHub.Domain.Models.IdentityUser;
using IdentityUserCredential = Kin.KinHub.KinHub.Domain.Models.IdentityUserCredential;
using IdentityUserProvider = Kin.KinHub.KinHub.Domain.Models.IdentityUserProvider;
using UserSession = Kin.KinHub.KinHub.Domain.Models.UserSession;

namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class KinHubAuthenticationService : IAuthenticationService
{
    private const int KinHubProviderId = 1;

    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityUserCredentialRepository _credentialRepository;
    private readonly IIdentityUserProviderRepository _userProviderRepository;
    private readonly IUserSessionRepository _sessionRepository;
    private readonly PasswordHasher<AspNetIdentityUser> _passwordHasher;

    public KinHubAuthenticationService(
        IIdentityUserRepository userRepository,
        IIdentityUserCredentialRepository credentialRepository,
        IIdentityUserProviderRepository userProviderRepository,
        IUserSessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _userProviderRepository = userProviderRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = new PasswordHasher<AspNetIdentityUser>();
    }

    /// <inheritdoc/>
    public async Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;

            var user = new DomainIdentityUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                DisplayName = request.DisplayName,
                IsEmailVerified = false,
                CreatedAt = now,
                UpdatedAt = now,
            };

            var created = await _userRepository.CreateAsync(user);

            var dummyUser = new AspNetIdentityUser { UserName = request.Email };
            var hash = _passwordHasher.HashPassword(dummyUser, request.Password);

            var credential = new IdentityUserCredential
            {
                Id = Guid.NewGuid(),
                UserId = created.Id,
                PasswordHash = hash,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _credentialRepository.CreateAsync(credential);

            var userProvider = new IdentityUserProvider
            {
                Id = Guid.NewGuid(),
                UserId = created.Id,
                ProviderId = KinHubProviderId,
                ProviderUserId = created.Email,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _userProviderRepository.CreateAsync(userProvider);

            return Result<RegisterResponse>.Success(new RegisterResponse
            {
                UserId = created.Id,
                Email = created.Email,
            });
        }
        catch (DuplicateEntityException ex)
        {
            return Result<RegisterResponse>.Conflict(ex.Message);
        }
        catch (DomainException)
        {
            return Result<RegisterResponse>.UnexpectedError("Registration failed. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userRepository.FindByEmailAsync(request.Email);

            if (users is null)
                return Result<LoginResponse>.Unauthorized("Invalid email or password.");

            var credential = await _credentialRepository.GetByUserIdAsync(users.Id);

            if (credential is null)
                return Result<LoginResponse>.Unauthorized("Invalid email or password.");

            var dummyUser = new AspNetIdentityUser { UserName = request.Email };
            var verificationResult = _passwordHasher.VerifyHashedPassword(
                dummyUser,
                credential.PasswordHash!,
                request.Password);

            if (verificationResult is PasswordVerificationResult.Failed)
                return Result<LoginResponse>.Unauthorized("Invalid email or password.");

            var now = DateTime.UtcNow;
            var session = new UserSession
            {
                Id = Guid.NewGuid(),
                UserId = users.Id,
                ProviderId = KinHubProviderId,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _sessionRepository.CreateAsync(session);

            return Result<LoginResponse>.Success(new LoginResponse
            {
                SessionId = session.Id,
                UserId = users.Id,
                Email = users.Email,
                DisplayName = users.DisplayName,
            });
        }
        catch (DomainException)
        {
            return Result<LoginResponse>.UnexpectedError("Login failed. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> LogoutAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _sessionRepository.DeleteAsync(sessionId);
            return Result<bool>.Success(true);
        }
        catch (EntityNotFoundException)
        {
            return Result<bool>.NotFound("Session not found.");
        }
        catch (DomainException)
        {
            return Result<bool>.UnexpectedError("Logout failed. Please try again.");
        }
    }
}
