using Kin.KinHub.Core.Business.Common;
using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Domain.Interfaces.Recipes;
using Kin.KinHub.Core.Domain.Recipes;

namespace Kin.KinHub.Core.Business;

public sealed class KinHubRecipeStepService : IRecipeStepService
{
    private readonly IRecipeStepRepository _recipeStepRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeBookRepository _recipeBookRepository;
    private readonly IFamilyRepository _familyRepository;

    public KinHubRecipeStepService(
        IRecipeStepRepository recipeStepRepository,
        IRecipeRepository recipeRepository,
        IRecipeBookRepository recipeBookRepository,
        IFamilyRepository familyRepository)
    {
        _recipeStepRepository = recipeStepRepository;
        _recipeRepository = recipeRepository;
        _recipeBookRepository = recipeBookRepository;
        _familyRepository = familyRepository;
    }

    public async Task<Result<RecipeStepResponse>> CreateAsync(
        CreateRecipeStepRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeStepResponse>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null)
            return Result<RecipeStepResponse>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeStepResponse>.Unauthorized("Access denied.");

        var now = DateTime.UtcNow;
        var step = new RecipeStep
        {
            Id = Guid.NewGuid(),
            Order = request.Order,
            Description = request.Description,
            RecipeId = request.RecipeId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var created = await _recipeStepRepository.AddAsync(step, cancellationToken);
        return Result<RecipeStepResponse>.Success(Map(created));
    }

    public async Task<Result<IReadOnlyList<RecipeStepResponse>>> GetAllAsync(
        Guid recipeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<IReadOnlyList<RecipeStepResponse>>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null)
            return Result<IReadOnlyList<RecipeStepResponse>>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<IReadOnlyList<RecipeStepResponse>>.Unauthorized("Access denied.");

        var steps = await _recipeStepRepository.GetAllByFamilyIdAsync(recipeId, cancellationToken);
        return Result<IReadOnlyList<RecipeStepResponse>>.Success(steps.Select(Map).ToList());
    }

    public async Task<Result<RecipeStepResponse>> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeStepResponse>.NotFound("Family not found for the current user.");

        var step = await _recipeStepRepository.GetByIdAsync(id, cancellationToken);
        if (step is null)
            return Result<RecipeStepResponse>.NotFound("Recipe step not found.");

        var recipe = await _recipeRepository.GetByIdAsync(step.RecipeId, cancellationToken);
        var book = recipe is not null ? await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken) : null;
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeStepResponse>.Unauthorized("Access denied.");

        return Result<RecipeStepResponse>.Success(Map(step));
    }

    public async Task<Result<RecipeStepResponse>> UpdateAsync(
        Guid id,
        UpdateRecipeStepRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeStepResponse>.NotFound("Family not found for the current user.");

        var step = await _recipeStepRepository.GetByIdAsync(id, cancellationToken);
        if (step is null)
            return Result<RecipeStepResponse>.NotFound("Recipe step not found.");

        var recipe = await _recipeRepository.GetByIdAsync(step.RecipeId, cancellationToken);
        var book = recipe is not null ? await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken) : null;
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeStepResponse>.Unauthorized("Access denied.");

        step.Order = request.Order;
        step.Description = request.Description;
        step.UpdatedAt = DateTime.UtcNow;

        var updated = await _recipeStepRepository.UpdateAsync(step, cancellationToken);
        return Result<RecipeStepResponse>.Success(Map(updated));
    }

    public async Task<Result<bool>> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<bool>.NotFound("Family not found for the current user.");

        var step = await _recipeStepRepository.GetByIdAsync(id, cancellationToken);
        if (step is null)
            return Result<bool>.NotFound("Recipe step not found.");

        var recipe = await _recipeRepository.GetByIdAsync(step.RecipeId, cancellationToken);
        var book = recipe is not null ? await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken) : null;
        if (book is null || book.FamilyId != family.Id)
            return Result<bool>.Unauthorized("Access denied.");

        await _recipeStepRepository.SoftDeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    private static RecipeStepResponse Map(RecipeStep s) =>
        new() { Id = s.Id, Order = s.Order, Description = s.Description, RecipeId = s.RecipeId };
}
