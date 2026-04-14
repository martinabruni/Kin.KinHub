using Kin.KinHub.Core.Domain.Recipes;

namespace Kin.KinHub.Core.Domain.Interfaces.Recipes;

/// <summary>
/// Repository contract for <see cref="Recipe"/> aggregates.
/// </summary>
public interface IRecipeRepository
{
    /// <summary>Returns the recipe matching the given id, or null if not found.</summary>
    Task<Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns all recipes belonging to the given recipe book.</summary>
    Task<IReadOnlyList<Recipe>> GetAllByFamilyIdAsync(Guid recipeBookId, CancellationToken cancellationToken = default);

    /// <summary>Persists a new recipe and returns it.</summary>
    Task<Recipe> AddAsync(Recipe recipe, CancellationToken cancellationToken = default);

    /// <summary>Updates the given recipe and returns the updated state.</summary>
    Task<Recipe> UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes the recipe with the given id.</summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
