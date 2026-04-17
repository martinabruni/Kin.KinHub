using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Api.RecipeFeature;

[ApiController]
[Route("api/fridges")]
public sealed class FridgeController : ControllerBase
{
    private readonly IFridgeService _fridgeService;
    private readonly IRequestValidator<CreateFridgeRequest> _createValidator;
    private readonly IRequestValidator<UpdateFridgeRequest> _updateValidator;
    private readonly ICurrentUser _currentUser;

    public FridgeController(
        IFridgeService fridgeService,
        IRequestValidator<CreateFridgeRequest> createValidator,
        IRequestValidator<UpdateFridgeRequest> updateValidator,
        ICurrentUser currentUser)
    {
        _fridgeService = fridgeService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateFridgeRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _fridgeService.CreateAsync(request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToCreatedActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _fridgeService.GetAllAsync(_currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _fridgeService.GetByIdAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateFridgeRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _fridgeService.UpdateAsync(id, request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _fridgeService.DeleteAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }
}
