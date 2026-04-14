using Kin.KinHub.Core.Api.Validators.Interfaces;
using Kin.KinHub.Core.Business;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Api.Controllers;

[ApiController]
[Route("api/recipe-books/{recipeBookId:guid}/recipes/{recipeId:guid}/ingredients")]
public sealed class RecipeIngredientController : ControllerBase
{
    private readonly IRecipeIngredientService _recipeIngredientService;
    private readonly IRequestValidator<CreateRecipeIngredientRequest> _createValidator;
    private readonly IRequestValidator<UpdateRecipeIngredientRequest> _updateValidator;
    private readonly ICurrentUser _currentUser;

    public RecipeIngredientController(
        IRecipeIngredientService recipeIngredientService,
        IRequestValidator<CreateRecipeIngredientRequest> createValidator,
        IRequestValidator<UpdateRecipeIngredientRequest> updateValidator,
        ICurrentUser currentUser)
    {
        _recipeIngredientService = recipeIngredientService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        Guid recipeBookId,
        Guid recipeId,
        [FromBody] CreateRecipeIngredientRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _recipeIngredientService.CreateAsync(request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToCreatedActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid recipeBookId, Guid recipeId, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _recipeIngredientService.GetAllAsync(recipeId, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid recipeBookId, Guid recipeId, Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _recipeIngredientService.GetByIdAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid recipeBookId,
        Guid recipeId,
        Guid id,
        [FromBody] UpdateRecipeIngredientRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _recipeIngredientService.UpdateAsync(id, request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid recipeBookId, Guid recipeId, Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _recipeIngredientService.DeleteAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }
}
