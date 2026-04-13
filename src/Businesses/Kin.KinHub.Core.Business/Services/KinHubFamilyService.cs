using BCrypt.Net;
using Kin.KinHub.Core.Business.Common;
using Kin.KinHub.Core.Domain;

namespace Kin.KinHub.Core.Business;

public sealed class KinHubFamilyService : IFamilyService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IFamilyMemberRepository _familyMemberRepository;
    private readonly IFamilyRoleRepository _familyRoleRepository;
    private readonly IMemberRoleRepository _memberRoleRepository;

    public KinHubFamilyService(
        IFamilyRepository familyRepository,
        IFamilyMemberRepository familyMemberRepository,
        IFamilyRoleRepository familyRoleRepository,
        IMemberRoleRepository memberRoleRepository)
    {
        _familyRepository = familyRepository;
        _familyMemberRepository = familyMemberRepository;
        _familyRoleRepository = familyRoleRepository;
        _memberRoleRepository = memberRoleRepository;
    }

    /// <inheritdoc/>
    public async Task<Result<CreateFamilyResponse>> CreateFamilyAsync(
        CreateFamilyRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
            if (existing is not null)
                return Result<CreateFamilyResponse>.Conflict("A family already exists for this user.");

            var now = DateTime.UtcNow;

            var family = new Family
            {
                Id = Guid.NewGuid(),
                Name = request.FamilyName,
                UserId = userId,
                AdminCodeHash = BCrypt.Net.BCrypt.HashPassword(request.AdminCode),
                CreatedAt = now,
                UpdatedAt = now,
            };

            var createdFamily = await _familyRepository.CreateAsync(family);

            var adminRole = await _familyRoleRepository.FindByRoleTypeAsync(FamilyRoleType.Admin, cancellationToken);
            if (adminRole is null)
                return Result<CreateFamilyResponse>.UnexpectedError("Admin role configuration is missing.");

            var memberRole = await _familyRoleRepository.FindByRoleTypeAsync(FamilyRoleType.Member, cancellationToken);
            if (memberRole is null)
                return Result<CreateFamilyResponse>.UnexpectedError("Member role configuration is missing.");

            var ownerMember = new FamilyMember
            {
                Id = Guid.NewGuid(),
                Name = request.OwnerProfileName,
                FamilyId = createdFamily.Id,
                CreatedAt = now,
                UpdatedAt = now,
            };

            var createdOwner = await _familyMemberRepository.CreateAsync(ownerMember);

            var ownerMemberRole = new MemberRole
            {
                Id = Guid.NewGuid(),
                MemberId = createdOwner.Id,
                RoleId = adminRole.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _memberRoleRepository.CreateAsync(ownerMemberRole);

            foreach (var memberName in request.AdditionalMembers)
            {
                var additionalMember = new FamilyMember
                {
                    Id = Guid.NewGuid(),
                    Name = memberName,
                    FamilyId = createdFamily.Id,
                    CreatedAt = now,
                    UpdatedAt = now,
                };

                var createdMember = await _familyMemberRepository.CreateAsync(additionalMember);

                var additionalMemberRole = new MemberRole
                {
                    Id = Guid.NewGuid(),
                    MemberId = createdMember.Id,
                    RoleId = memberRole.Id,
                    IsActive = true,
                    CreatedAt = now,
                    UpdatedAt = now,
                };

                await _memberRoleRepository.CreateAsync(additionalMemberRole);
            }

            return Result<CreateFamilyResponse>.Success(new CreateFamilyResponse
            {
                FamilyId = createdFamily.Id,
                AdminMemberId = createdOwner.Id,
            });
        }
        catch (DuplicateEntityException ex)
        {
            return Result<CreateFamilyResponse>.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<CreateFamilyResponse>.UnexpectedError(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<AddFamilyMemberResponse>> AddFamilyMemberAsync(
        Guid familyId,
        AddFamilyMemberRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
            if (family is null)
                return Result<AddFamilyMemberResponse>.NotFound("Family not found for this user.");

            if (family.Id != familyId)
                return Result<AddFamilyMemberResponse>.Unauthorized("You do not own this family.");

            var memberRole = await _familyRoleRepository.FindByRoleTypeAsync(FamilyRoleType.Member, cancellationToken);
            if (memberRole is null)
                return Result<AddFamilyMemberResponse>.UnexpectedError("Member role configuration is missing.");

            var now = DateTime.UtcNow;

            var newMember = new FamilyMember
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                FamilyId = familyId,
                CreatedAt = now,
                UpdatedAt = now,
            };

            var createdMember = await _familyMemberRepository.CreateAsync(newMember);

            var memberRoleAssignment = new MemberRole
            {
                Id = Guid.NewGuid(),
                MemberId = createdMember.Id,
                RoleId = memberRole.Id,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
            };

            await _memberRoleRepository.CreateAsync(memberRoleAssignment);

            return Result<AddFamilyMemberResponse>.Success(new AddFamilyMemberResponse
            {
                MemberId = createdMember.Id,
            });
        }
        catch (DuplicateEntityException ex)
        {
            return Result<AddFamilyMemberResponse>.Conflict(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return Result<AddFamilyMemberResponse>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<AddFamilyMemberResponse>.UnexpectedError(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<FamilyDetailResponse>> GetFamilyAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
            if (family is null)
                return Result<FamilyDetailResponse>.NotFound("Family not found for this user.");

            var members = await _familyMemberRepository.GetByFamilyIdAsync(family.Id, cancellationToken);
            var allRoles = await _familyRoleRepository.GetAllAsync();

            var memberDtos = new List<FamilyMemberDto>(members.Count);

            foreach (var member in members)
            {
                var memberRoles = await _memberRoleRepository.GetByMemberIdAsync(member.Id, cancellationToken);
                var primaryRole = memberRoles.FirstOrDefault();
                var roleName = primaryRole is not null
                    ? allRoles.FirstOrDefault(r => r.Id == primaryRole.RoleId)?.Name ?? string.Empty
                    : string.Empty;

                memberDtos.Add(new FamilyMemberDto
                {
                    Id = member.Id,
                    Name = member.Name,
                    Role = roleName,
                });
            }

            return Result<FamilyDetailResponse>.Success(new FamilyDetailResponse
            {
                FamilyId = family.Id,
                Name = family.Name,
                Members = memberDtos,
            });
        }
        catch (Exception ex)
        {
            return Result<FamilyDetailResponse>.UnexpectedError(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> VerifyAdminCodeAsync(
        Guid familyId,
        string adminCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var family = await _familyRepository.GetAsync(familyId);
            if (!BCrypt.Net.BCrypt.Verify(adminCode, family.AdminCodeHash))
                return Result<bool>.ValidationError("Codice admin non corretto.");

            return Result<bool>.Success(true);
        }
        catch (EntityNotFoundException ex)
        {
            return Result<bool>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<bool>.UnexpectedError(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<bool>> DeleteFamilyMemberAsync(
        Guid familyId,
        Guid memberId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
            if (family is null)
                return Result<bool>.NotFound("Family not found for this user.");

            if (family.Id != familyId)
                return Result<bool>.Unauthorized("You do not own this family.");

            var member = await _familyMemberRepository.GetAsync(memberId);

            if (member.FamilyId != familyId)
                return Result<bool>.NotFound("Member not found in this family.");

            member.IsDeleted = true;
            member.UpdatedAt = DateTime.UtcNow;
            await _familyMemberRepository.UpdateAsync(member.Id, member);

            return Result<bool>.Success(true);
        }
        catch (EntityNotFoundException ex)
        {
            return Result<bool>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<bool>.UnexpectedError(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<UpdateFamilyMemberResponse>> UpdateFamilyMemberAsync(
        Guid familyId,
        Guid memberId,
        UpdateFamilyMemberRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
            if (family is null)
                return Result<UpdateFamilyMemberResponse>.NotFound("Family not found for this user.");

            if (family.Id != familyId)
                return Result<UpdateFamilyMemberResponse>.Unauthorized("You do not own this family.");

            var member = await _familyMemberRepository.GetAsync(memberId);

            if (member.FamilyId != familyId || member.IsDeleted)
                return Result<UpdateFamilyMemberResponse>.NotFound("Member not found in this family.");

            var existing = await _familyMemberRepository.FindByNameAsync(familyId, request.Name, cancellationToken);
            if (existing is not null && existing.Id != memberId)
                return Result<UpdateFamilyMemberResponse>.Conflict("A member with this name already exists in the family.");

            member.Name = request.Name;
            member.UpdatedAt = DateTime.UtcNow;
            await _familyMemberRepository.UpdateAsync(member.Id, member);

            return Result<UpdateFamilyMemberResponse>.Success(new UpdateFamilyMemberResponse
            {
                Id = member.Id,
                Name = member.Name,
            });
        }
        catch (EntityNotFoundException ex)
        {
            return Result<UpdateFamilyMemberResponse>.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<UpdateFamilyMemberResponse>.UnexpectedError(ex.Message);
        }
    }
}
