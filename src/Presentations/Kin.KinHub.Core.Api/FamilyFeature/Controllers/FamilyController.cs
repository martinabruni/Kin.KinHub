using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Api.FamilyFeature;

[ApiController]
[Route("api/families")]
public sealed class FamilyController : ControllerBase
{
    private readonly IFamilyService _familyService;
    private readonly IRequestValidator<CreateFamilyRequest> _createValidator;
    private readonly IRequestValidator<AddFamilyMemberRequest> _addMemberValidator;
    private readonly IRequestValidator<UpdateFamilyMemberRequest> _updateMemberValidator;
    private readonly IRequestValidator<VerifyAdminCodeRequest> _verifyAdminCodeValidator;
    private readonly IRequestValidator<UpdateFamilyRequest> _updateFamilyValidator;
    private readonly IRequestValidator<UpdateAdminCodeRequest> _updateAdminCodeValidator;
    private readonly ICurrentUser _currentUser;

    public FamilyController(
        IFamilyService familyService,
        IRequestValidator<CreateFamilyRequest> createValidator,
        IRequestValidator<AddFamilyMemberRequest> addMemberValidator,
        IRequestValidator<UpdateFamilyMemberRequest> updateMemberValidator,
        IRequestValidator<VerifyAdminCodeRequest> verifyAdminCodeValidator,
        IRequestValidator<UpdateFamilyRequest> updateFamilyValidator,
        IRequestValidator<UpdateAdminCodeRequest> updateAdminCodeValidator,
        ICurrentUser currentUser)
    {
        _familyService = familyService;
        _createValidator = createValidator;
        _addMemberValidator = addMemberValidator;
        _updateMemberValidator = updateMemberValidator;
        _verifyAdminCodeValidator = verifyAdminCodeValidator;
        _updateFamilyValidator = updateFamilyValidator;
        _updateAdminCodeValidator = updateAdminCodeValidator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateFamilyRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _familyService.CreateFamilyAsync(request, _currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToCreatedActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _familyService.GetFamilyAsync(_currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPost("{familyId:guid}/members")]
    public async Task<IActionResult> AddMemberAsync(
        Guid familyId,
        [FromBody] AddFamilyMemberRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _addMemberValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _familyService.AddFamilyMemberAsync(familyId, request, _currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToCreatedActionResult(result);
    }

    [HttpPost("{familyId:guid}/verify-admin-code")]
    public async Task<IActionResult> VerifyAdminCodeAsync(
        Guid familyId,
        [FromBody] VerifyAdminCodeRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _verifyAdminCodeValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _familyService.VerifyAdminCodeAsync(familyId, request.AdminCode, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpDelete("{familyId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> DeleteMemberAsync(
        Guid familyId,
        Guid memberId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _familyService.DeleteFamilyMemberAsync(familyId, memberId, _currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPut("{familyId:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> UpdateMemberAsync(
        Guid familyId,
        Guid memberId,
        [FromBody] UpdateFamilyMemberRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateMemberValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _familyService.UpdateFamilyMemberAsync(familyId, memberId, request, _currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPatch("{familyId:guid}")]
    public async Task<IActionResult> UpdateFamilyAsync(
        Guid familyId,
        [FromBody] UpdateFamilyRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateFamilyValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _familyService.UpdateFamilyAsync(familyId, request, _currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPatch("{familyId:guid}/admin-code")]
    public async Task<IActionResult> UpdateAdminCodeAsync(
        Guid familyId,
        [FromBody] UpdateAdminCodeRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateAdminCodeValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _familyService.UpdateAdminCodeAsync(familyId, request, _currentUser.UserId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
