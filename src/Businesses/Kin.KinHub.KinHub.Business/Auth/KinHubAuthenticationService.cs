using Kin.KinHub.KinHub.Business.Common;
using Kin.KinHub.KinHub.Domain;
using Kin.KinHub.KinHub.Domain.Common;
using Kin.KinHub.KinHub.Domain.Interfaces;
using Kin.KinHub.KinHub.Domain.Models;

namespace Kin.KinHub.KinHub.Business.Auth;

public sealed class KinHubAuthenticationService : IAuthenticationService
{
    private const int KinHubProviderId = 1;

    private readonly IIdentityUserRepository _userRepository;
    private readonly IIdentityUserCredentialRepository _credentialRepository;
    private readonly IIdentityUserProviderRepository _userProviderRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public KinHubAuthenticationService(
        IIdentityUserRepository userRepository,
        IIdentityUserCredentialRepository credentialRepository,
        IIdentityUserProviderRepository userProviderRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _userProviderRepository = userProviderRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    /// <inheritdoc/>
    public async Task<Result<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;

            var user = new IdentityUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                DisplayName = request.DisplayName,
                IsEmailVerified = false,
                Roles = ["user"],
                Status = UserStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
            };

            var created = await _userRepository.CreateAsync(user);

            var hash = _passwordHasher.Hash(request.Password);

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
                ProviderUserId = created.Id.ToString(),
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
            var user = await _userRepository.FindByEmailAsync(request.Email);

            if (user is null)
                return Result<LoginResponse>.Unauthorized("Invalid email or password.");

            if (user.Status is not UserStatus.Active)
                return Result<LoginResponse>.Unauthorized("Account is not active.");

            var credential = await _credentialRepository.GetByUserIdAsync(user.Id);

            if (credential?.PasswordHash is null)
                return Result<LoginResponse>.Unauthorized("Invalid email or password.");

            if (!_passwordHasher.Verify(request.Password, credential.PasswordHash))
                return Result<LoginResponse>.Unauthorized("Invalid email or password.");

            return Result<LoginResponse>.Success(await GenerateTokenResponseAsync(user));
        }
        catch (DomainException)
        {
            return Result<LoginResponse>.UnexpectedError("Login failed. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<LoginResponse>> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stored = await _refreshTokenRepository.FindByTokenAsync(refreshToken);

            if (stored is null || stored.Revoked || stored.ExpiresAtUtc <= DateTime.UtcNow)
                return Result<LoginResponse>.Unauthorized("Invalid or expired refresh token.");

            stored.Revoked = true;
            await _refreshTokenRepository.UpdateAsync(stored.Id, stored);

            var user = await _userRepository.GetAsync(stored.UserId);

            if (user.Status is not UserStatus.Active)
                return Result<LoginResponse>.Unauthorized("Account is not active.");

            return Result<LoginResponse>.Success(await GenerateTokenResponseAsync(user));
        }
        catch (EntityNotFoundException)
        {
            return Result<LoginResponse>.Unauthorized("Invalid refresh token.");
        }
        catch (DomainException)
        {
            return Result<LoginResponse>.UnexpectedError("Token refresh failed. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stored = await _refreshTokenRepository.FindByTokenAsync(refreshToken);

            if (stored is null)
                return Result<bool>.NotFound("Refresh token not found.");

            stored.Revoked = true;
            await _refreshTokenRepository.UpdateAsync(stored.Id, stored);

            return Result<bool>.Success(true);
        }
        catch (DomainException)
        {
            return Result<bool>.UnexpectedError("Logout failed. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<UserProfileResponse>> GetCurrentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetAsync(userId);

            return Result<UserProfileResponse>.Success(new UserProfileResponse
            {
                UserId = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Roles = user.Roles,
            });
        }
        catch (EntityNotFoundException)
        {
            return Result<UserProfileResponse>.NotFound("User not found.");
        }
        catch (DomainException)
        {
            return Result<UserProfileResponse>.UnexpectedError("Failed to retrieve user profile.");
        }
    }

    private async Task<LoginResponse> GenerateTokenResponseAsync(IdentityUser user)
    {
        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        var rawRefreshToken = _tokenGenerator.GenerateRefreshToken();

        var now = DateTime.UtcNow;
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = rawRefreshToken,
            ExpiresAtUtc = now.AddDays(7),
            Revoked = false,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken,
            ExpiresIn = _tokenGenerator.AccessTokenExpirySeconds,
            Email = user.Email,
            DisplayName = user.DisplayName,
        };
    }
}
