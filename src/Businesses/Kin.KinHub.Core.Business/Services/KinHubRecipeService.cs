using Kin.KinHub.Core.Business.Common;
using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Domain.Interfaces.Recipes;
using Kin.KinHub.Core.Domain.Recipes;

namespace Kin.KinHub.Core.Business;

public sealed class KinHubRecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeBookRepository _recipeBookRepository;
    private readonly IRecipeIngredientRepository _recipeIngredientRepository;
    private readonly IRecipeStepRepository _recipeStepRepository;
    private readonly IFamilyRepository _familyRepository;

    public KinHubRecipeService(
        IRecipeRepository recipeRepository,
        IRecipeBookRepository recipeBookRepository,
        IRecipeIngredientRepository recipeIngredientRepository,
        IRecipeStepRepository recipeStepRepository,
        IFamilyRepository familyRepository)
    {
        _recipeRepository = recipeRepository;
        _recipeBookRepository = recipeBookRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
        _recipeStepRepository = recipeStepRepository;
        _familyRepository = familyRepository;
    }

    public async Task<Result<RecipeResponse>> CreateAsync(
        CreateRecipeRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeResponse>.NotFound("Family not found for the current user.");

        var book = await _recipeBookRepository.GetByIdAsync(request.RecipeBookId, cancellationToken);
        if (book is null)
            return Result<RecipeResponse>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<RecipeResponse>.Unauthorized("Access denied.");

        var now = DateTime.UtcNow;
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Backstory = request.Backstory,
            FinalTime = request.FinalTime,
            Portions = request.Portions,
            RecipeBookId = request.RecipeBookId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var created = await _recipeRepository.AddAsync(recipe, cancellationToken);

        if (request.Ingredients is { Count: > 0 })
        {
            var now2 = DateTime.UtcNow;
            foreach (var ing in request.Ingredients)
            {
                await _recipeIngredientRepository.AddAsync(new RecipeIngredient
                {
                    Id = Guid.NewGuid(),
                    Name = ing.Name,
                    MeasureUnit = ing.MeasureUnit,
                    Quantity = ing.Quantity,
                    RecipeId = created.Id,
                    CreatedAt = now2,
                    UpdatedAt = now2,
                }, cancellationToken);
            }
        }

        if (request.Steps is { Count: > 0 })
        {
            var now3 = DateTime.UtcNow;
            foreach (var step in request.Steps)
            {
                await _recipeStepRepository.AddAsync(new RecipeStep
                {
                    Id = Guid.NewGuid(),
                    Order = step.Order,
                    Description = step.Description,
                    RecipeId = created.Id,
                    CreatedAt = now3,
                    UpdatedAt = now3,
                }, cancellationToken);
            }
        }

        return Result<RecipeResponse>.Success(await MapAsync(created, cancellationToken));
    }

    public async Task<Result<IReadOnlyList<RecipeResponse>>> GetAllAsync(
        Guid recipeBookId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<IReadOnlyList<RecipeResponse>>.NotFound("Family not found for the current user.");

        var book = await _recipeBookRepository.GetByIdAsync(recipeBookId, cancellationToken);
        if (book is null)
            return Result<IReadOnlyList<RecipeResponse>>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<IReadOnlyList<RecipeResponse>>.Unauthorized("Access denied.");

        var recipes = await _recipeRepository.GetAllByFamilyIdAsync(recipeBookId, cancellationToken);
        var responses = new List<RecipeResponse>();
        foreach (var r in recipes)
            responses.Add(await MapAsync(r, cancellationToken));

        return Result<IReadOnlyList<RecipeResponse>>.Success(responses);
    }

    public async Task<Result<RecipeResponse>> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeResponse>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(id, cancellationToken);
        if (recipe is null)
            return Result<RecipeResponse>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeResponse>.Unauthorized("Access denied.");

        return Result<RecipeResponse>.Success(await MapAsync(recipe, cancellationToken));
    }

    public async Task<Result<RecipeResponse>> UpdateAsync(
        Guid id,
        UpdateRecipeRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeResponse>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(id, cancellationToken);
        if (recipe is null)
            return Result<RecipeResponse>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeResponse>.Unauthorized("Access denied.");

        recipe.Name = request.Name;
        recipe.Backstory = request.Backstory;
        recipe.FinalTime = request.FinalTime;
        recipe.Portions = request.Portions;
        recipe.UpdatedAt = DateTime.UtcNow;

        var updated = await _recipeRepository.UpdateAsync(recipe, cancellationToken);
        return Result<RecipeResponse>.Success(await MapAsync(updated, cancellationToken));
    }

    public async Task<Result<bool>> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<bool>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(id, cancellationToken);
        if (recipe is null)
            return Result<bool>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<bool>.Unauthorized("Access denied.");

        await _recipeRepository.SoftDeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    private async Task<RecipeResponse> MapAsync(Recipe r, CancellationToken cancellationToken)
    {
        var ingredients = await _recipeIngredientRepository.GetAllByFamilyIdAsync(r.Id, cancellationToken);
        var steps = await _recipeStepRepository.GetAllByFamilyIdAsync(r.Id, cancellationToken);

        return new RecipeResponse
        {
            Id = r.Id,
            Name = r.Name,
            Backstory = r.Backstory,
            FinalTime = r.FinalTime,
            Portions = r.Portions,
            RecipeBookId = r.RecipeBookId,
            Ingredients = ingredients.Select(i => new RecipeIngredientResponse
            {
                Id = i.Id,
                Name = i.Name,
                MeasureUnit = i.MeasureUnit,
                Quantity = i.Quantity,
                RecipeId = i.RecipeId,
            }).ToList(),
            Steps = steps.Select(s => new RecipeStepResponse
            {
                Id = s.Id,
                Order = s.Order,
                Description = s.Description,
                RecipeId = s.RecipeId,
            }).ToList(),
        };
    }
}
