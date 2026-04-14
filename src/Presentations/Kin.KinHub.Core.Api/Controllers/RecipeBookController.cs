using Kin.KinHub.Core.Api.Validators.Interfaces;
using Kin.KinHub.Core.Business;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kin.KinHub.Core.Api.Controllers;

[ApiController]
[Route("api/recipe-books")]
public sealed class RecipeBookController : ControllerBase
{
    private readonly IRecipeBookService _recipeBookService;
    private readonly IRequestValidator<CreateRecipeBookRequest> _createValidator;
    private readonly IRequestValidator<UpdateRecipeBookRequest> _updateValidator;
    private readonly ICurrentUser _currentUser;

    public RecipeBookController(
        IRecipeBookService recipeBookService,
        IRequestValidator<CreateRecipeBookRequest> createValidator,
        IRequestValidator<UpdateRecipeBookRequest> updateValidator,
        ICurrentUser currentUser)
    {
        _recipeBookService = recipeBookService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateRecipeBookRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _recipeBookService.CreateAsync(request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToCreatedActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _recipeBookService.GetAllAsync(_currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _recipeBookService.GetByIdAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateRecipeBookRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        if (request is null)
            return BadRequest(new { message = "Invalid request body." });

        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors });

        var result = await _recipeBookService.UpdateAsync(id, request, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized(new { message = "Missing or invalid Authorization header." });

        var result = await _recipeBookService.DeleteAsync(id, _currentUser.UserId, cancellationToken);
        return HttpResultMapper.ToActionResult(result);
    }
}
