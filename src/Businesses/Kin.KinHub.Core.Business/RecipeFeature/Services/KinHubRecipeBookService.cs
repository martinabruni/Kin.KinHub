using Kin.KinHub.Core.Business.Common;

namespace Kin.KinHub.Core.Business.RecipeFeature;

public sealed class KinHubRecipeBookService : IRecipeBookService
{
    private readonly IRecipeBookRepository _recipeBookRepository;
    private readonly IFamilyRepository _familyRepository;

    public KinHubRecipeBookService(IRecipeBookRepository recipeBookRepository, IFamilyRepository familyRepository)
    {
        _recipeBookRepository = recipeBookRepository;
        _familyRepository = familyRepository;
    }

    public async Task<Result<RecipeBookResponse>> CreateAsync(
        CreateRecipeBookRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeBookResponse>.NotFound("Family not found for the current user.");

        var now = DateTime.UtcNow;
        var recipeBook = new RecipeBook
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            FamilyId = family.Id,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var created = await _recipeBookRepository.AddAsync(recipeBook, cancellationToken);
        return Result<RecipeBookResponse>.Success(Map(created));
    }

    public async Task<Result<IReadOnlyList<RecipeBookResponse>>> GetAllAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<IReadOnlyList<RecipeBookResponse>>.NotFound("Family not found for the current user.");

        var books = await _recipeBookRepository.GetAllByFamilyIdAsync(family.Id, cancellationToken);
        return Result<IReadOnlyList<RecipeBookResponse>>.Success(books.Select(Map).ToList());
    }

    public async Task<Result<RecipeBookResponse>> GetByIdAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeBookResponse>.NotFound("Family not found for the current user.");

        var book = await _recipeBookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return Result<RecipeBookResponse>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<RecipeBookResponse>.Unauthorized("Access denied.");

        return Result<RecipeBookResponse>.Success(Map(book));
    }

    public async Task<Result<RecipeBookResponse>> UpdateAsync(
        Guid id,
        UpdateRecipeBookRequest request,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeBookResponse>.NotFound("Family not found for the current user.");

        var book = await _recipeBookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return Result<RecipeBookResponse>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<RecipeBookResponse>.Unauthorized("Access denied.");

        book.Name = request.Name;
        book.Description = request.Description;
        book.UpdatedAt = DateTime.UtcNow;

        var updated = await _recipeBookRepository.UpdateAsync(book, cancellationToken);
        return Result<RecipeBookResponse>.Success(Map(updated));
    }

    public async Task<Result<bool>> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<bool>.NotFound("Family not found for the current user.");

        var book = await _recipeBookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return Result<bool>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<bool>.Unauthorized("Access denied.");

        await _recipeBookRepository.SoftDeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    private static RecipeBookResponse Map(RecipeBook b) =>
        new() { Id = b.Id, Name = b.Name, Description = b.Description, FamilyId = b.FamilyId };
}
