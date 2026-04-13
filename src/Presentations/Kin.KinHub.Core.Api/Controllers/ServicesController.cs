using Kin.KinHub.Core.Api.Validators.Interfaces;
using Kin.KinHub.Core.Business;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Api.Controllers;

[ApiController]
[Route("api/services")]
public sealed class ServicesController : ControllerBase
{
    private readonly IKinHubServiceService _serviceService;
    private readonly IRequestValidator<ToggleFamilyServiceRequest> _toggleValidator;
    private readonly ICurrentUser _currentUser;

    public ServicesController(
        IKinHubServiceService serviceService,
        IRequestValidator<ToggleFamilyServiceRequest> toggleValidator,
        ICurrentUser currentUser)
    {
        _serviceService = serviceService;
        _toggleValidator = toggleValidator;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _serviceService.GetAllServicesAsync(cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpGet("family/{familyId:guid}")]
    public async Task<IActionResult> GetFamilyServicesAsync(
        Guid familyId,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _serviceService.GetFamilyServicesAsync(familyId, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPost("family/{familyId:guid}/toggle")]
    public async Task<IActionResult> ToggleAsync(
        Guid familyId,
        [FromBody] ToggleFamilyServiceRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _toggleValidator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _serviceService.ToggleFamilyServiceAsync(familyId, request, cancellationToken);

        return HttpResultMapper.ToActionResult(result);
    }
}
