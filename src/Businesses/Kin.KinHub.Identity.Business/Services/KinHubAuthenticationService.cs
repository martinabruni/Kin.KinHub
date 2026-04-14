using Kin.KinHub.Identity.Business.Enums;
using Kin.KinHub.Identity.Business.Interfaces;
using Kin.KinHub.Identity.Business.Models;
using Kin.KinHub.Identity.Domain.Enums;
using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Interfaces;
using Kin.KinHub.Identity.Domain.Models;

namespace Kin.KinHub.Identity.Business.Services;

public sealed class KinHubAuthenticationService : IAuthenticationService
{
    private readonly IKinUserRepository _userRepository;
    private readonly IUserCredentialRepository _credentialRepository;
    private readonly IUserProviderRepository _userProviderRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public KinHubAuthenticationService(
        IKinUserRepository userRepository,
        IUserCredentialRepository credentialRepository,
        IUserProviderRepository userProviderRepository,
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

            var user = new KinUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                DisplayName = request.DisplayName,
                IsEmailVerified = false,
                Status = UserStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
            };

            var created = await _userRepository.CreateAsync(user);

            var hash = _passwordHasher.Hash(request.Password);

            var credential = new UserCredential
            {
                Id = Guid.NewGuid(),
                UserId = created.Id,
                PasswordHash = hash,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _credentialRepository.CreateAsync(credential);

            var userProvider = new UserProvider
            {
                Id = Guid.NewGuid(),
                UserId = created.Id,
                ProviderId = (int)IdentityProviderType.KinHub,
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

    private async Task<LoginResponse> GenerateTokenResponseAsync(KinUser user)
    {
        var accessToken = _tokenGenerator.GenerateAccessToken(user, []);
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

    /// <inheritdoc/>
    public async Task<Result<bool>> UpdateUserEmailAsync(
        Guid userId,
        UpdateUserEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetAsync(userId);

            var existing = await _userRepository.FindByEmailAsync(request.NewEmail);
            if (existing is not null && existing.Id != userId)
                return Result<bool>.Conflict("Email already in use.");

            user.Email = request.NewEmail;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user.Id, user);

            return Result<bool>.Success(true);
        }
        catch (EntityNotFoundException)
        {
            return Result<bool>.NotFound("User not found.");
        }
        catch (DomainException)
        {
            return Result<bool>.UnexpectedError("Failed to update email.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> UpdateUserPasswordAsync(
        Guid userId,
        UpdateUserPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var credential = await _credentialRepository.GetByUserIdAsync(userId);

            if (credential?.PasswordHash is null)
                return Result<bool>.Unauthorized("Invalid current password.");

            if (!_passwordHasher.Verify(request.CurrentPassword, credential.PasswordHash))
                return Result<bool>.Unauthorized("Invalid current password.");

            credential.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            credential.UpdatedAt = DateTime.UtcNow;
            await _credentialRepository.UpdateAsync(credential.Id, credential);

            return Result<bool>.Success(true);
        }
        catch (EntityNotFoundException)
        {
            return Result<bool>.NotFound("User not found.");
        }
        catch (DomainException)
        {
            return Result<bool>.UnexpectedError("Failed to update password.");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetAsync(userId);

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user.Id, user);

            return Result<bool>.Success(true);
        }
        catch (EntityNotFoundException)
        {
            return Result<bool>.NotFound("User not found.");
        }
        catch (DomainException)
        {
            return Result<bool>.UnexpectedError("Failed to delete account.");
        }
    }
}
