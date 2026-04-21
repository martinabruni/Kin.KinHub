using Kin.KinHub.Core.Business.Common;

namespace Kin.KinHub.Core.Business.RecipeFeature;

public sealed class KinHubRecipeIngredientService : IRecipeIngredientService
{
    private readonly IRecipeIngredientRepository _recipeIngredientRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeBookRepository _recipeBookRepository;
    private readonly IFamilyRepository _familyRepository;
    private readonly IEmbeddingService _embeddingService;

    public KinHubRecipeIngredientService(
        IRecipeIngredientRepository recipeIngredientRepository,
        IRecipeRepository recipeRepository,
        IRecipeBookRepository recipeBookRepository,
        IFamilyRepository familyRepository,
        IEmbeddingService embeddingService)
    {
        _recipeIngredientRepository = recipeIngredientRepository;
        _recipeRepository = recipeRepository;
        _recipeBookRepository = recipeBookRepository;
        _familyRepository = familyRepository;
        _embeddingService = embeddingService;
    }

    public async Task<Result<RecipeIngredientResponse>> CreateAsync(
        CreateRecipeIngredientRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeIngredientResponse>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null)
            return Result<RecipeIngredientResponse>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeIngredientResponse>.Unauthorized("Access denied.");

        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Name, cancellationToken);

        var now = DateTime.UtcNow;
        var ingredient = new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            MeasureUnit = request.MeasureUnit,
            Quantity = request.Quantity,
            RecipeId = request.RecipeId,
            Embedding = embedding,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var created = await _recipeIngredientRepository.AddAsync(ingredient, cancellationToken);
        return Result<RecipeIngredientResponse>.Success(Map(created));
    }

    public async Task<Result<IReadOnlyList<RecipeIngredientResponse>>> GetAllAsync(
        Guid recipeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<IReadOnlyList<RecipeIngredientResponse>>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null)
            return Result<IReadOnlyList<RecipeIngredientResponse>>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null || book.FamilyId != family.Id)
            return Result<IReadOnlyList<RecipeIngredientResponse>>.Unauthorized("Access denied.");

        var ingredients = await _recipeIngredientRepository.GetAllByFamilyIdAsync(recipeId, cancellationToken);
        return Result<IReadOnlyList<RecipeIngredientResponse>>.Success(ingredients.Select(Map).ToList());
    }

    public async Task<Result<RecipeIngredientResponse>> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeIngredientResponse>.NotFound("Family not found for the current user.");

        var ingredient = await _recipeIngredientRepository.GetByIdAsync(id, cancellationToken);
        if (ingredient is null)
            return Result<RecipeIngredientResponse>.NotFound("Recipe ingredient not found.");

        var recipe = await _recipeRepository.GetByIdAsync(ingredient.RecipeId, cancellationToken);
        var book = recipe is not null ? await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken) : null;
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeIngredientResponse>.Unauthorized("Access denied.");

        return Result<RecipeIngredientResponse>.Success(Map(ingredient));
    }

    public async Task<Result<RecipeIngredientResponse>> UpdateAsync(
        Guid id,
        UpdateRecipeIngredientRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeIngredientResponse>.NotFound("Family not found for the current user.");

        var ingredient = await _recipeIngredientRepository.GetByIdAsync(id, cancellationToken);
        if (ingredient is null)
            return Result<RecipeIngredientResponse>.NotFound("Recipe ingredient not found.");

        var recipe = await _recipeRepository.GetByIdAsync(ingredient.RecipeId, cancellationToken);
        var book = recipe is not null ? await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken) : null;
        if (book is null || book.FamilyId != family.Id)
            return Result<RecipeIngredientResponse>.Unauthorized("Access denied.");

        var nameChanged = !ingredient.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase);
        ingredient.Name = request.Name;
        ingredient.MeasureUnit = request.MeasureUnit;
        ingredient.Quantity = request.Quantity;
        ingredient.UpdatedAt = DateTime.UtcNow;

        if (nameChanged)
            ingredient.Embedding = await _embeddingService.GenerateEmbeddingAsync(request.Name, cancellationToken);

        var updated = await _recipeIngredientRepository.UpdateAsync(ingredient, cancellationToken);
        return Result<RecipeIngredientResponse>.Success(Map(updated));
    }

    public async Task<Result<bool>> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<bool>.NotFound("Family not found for the current user.");

        var ingredient = await _recipeIngredientRepository.GetByIdAsync(id, cancellationToken);
        if (ingredient is null)
            return Result<bool>.NotFound("Recipe ingredient not found.");

        var recipe = await _recipeRepository.GetByIdAsync(ingredient.RecipeId, cancellationToken);
        var book = recipe is not null ? await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken) : null;
        if (book is null || book.FamilyId != family.Id)
            return Result<bool>.Unauthorized("Access denied.");

        await _recipeIngredientRepository.SoftDeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    private static RecipeIngredientResponse Map(RecipeIngredient i) =>
        new() { Id = i.Id, Name = i.Name, MeasureUnit = i.MeasureUnit, Quantity = i.Quantity, RecipeId = i.RecipeId };
}
