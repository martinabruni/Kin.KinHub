using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Api.RecipeFeature;

[ApiController]
[Route("api/fridges/{fridgeId:guid}/ingredients")]
public sealed class FridgeIngredientController : ControllerBase
{
    private readonly IFridgeIngredientService _fridgeIngredientService;
    private readonly IRequestValidator<CreateFridgeIngredientRequest> _createValidator;
    private readonly IRequestValidator<UpdateFridgeIngredientRequest> _updateValidator;
    private readonly ICurrentUser _currentUser;

    public FridgeIngredientController(
        IFridgeIngredientService fridgeIngredientService,
        IRequestValidator<CreateFridgeIngredientRequest> createValidator,
        IRequestValidator<UpdateFridgeIngredientRequest> updateValidator,
        ICurrentUser currentUser)
    {
        _fridgeIngredientService = fridgeIngredientService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        Guid fridgeId,
        [FromBody] CreateFridgeIngredientRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _fridgeIngredientService.CreateAsync(request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToCreatedActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid fridgeId, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _fridgeIngredientService.GetAllAsync(fridgeId, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid fridgeId, Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _fridgeIngredientService.GetByIdAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid fridgeId,
        Guid id,
        [FromBody] UpdateFridgeIngredientRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _fridgeIngredientService.UpdateAsync(id, request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid fridgeId, Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _fridgeIngredientService.DeleteAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }
}
