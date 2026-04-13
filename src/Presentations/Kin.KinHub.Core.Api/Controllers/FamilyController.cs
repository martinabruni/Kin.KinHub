using Kin.KinHub.Core.Api.Validators.Interfaces;
using Kin.KinHub.Core.Business;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Kin.KinHub.Core.Api.Controllers;

[ApiController]
[Route("api/families")]
public sealed class FamilyController : ControllerBase
{
    private readonly IFamilyService _familyService;
    private readonly IRequestValidator<CreateFamilyRequest> _createValidator;
    private readonly IRequestValidator<AddFamilyMemberRequest> _addMemberValidator;
    private readonly ICurrentUser _currentUser;

    public FamilyController(
        IFamilyService familyService,
        IRequestValidator<CreateFamilyRequest> createValidator,
        IRequestValidator<AddFamilyMemberRequest> addMemberValidator,
        ICurrentUser currentUser)
    {
        _familyService = familyService;
        _createValidator = createValidator;
        _addMemberValidator = addMemberValidator;
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
}
