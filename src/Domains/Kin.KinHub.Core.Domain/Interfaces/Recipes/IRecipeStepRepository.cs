using Kin.KinHub.Core.Domain.Recipes;

namespace Kin.KinHub.Core.Domain.Interfaces.Recipes;

/// <summary>
/// Repository contract for <see cref="RecipeStep"/> aggregates.
/// </summary>
public interface IRecipeStepRepository
{
    /// <summary>Returns the recipe step matching the given id, or null if not found.</summary>
    Task<RecipeStep?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Returns all steps belonging to the given recipe.</summary>
    Task<IReadOnlyList<RecipeStep>> GetAllByFamilyIdAsync(Guid recipeId, CancellationToken cancellationToken = default);

    /// <summary>Persists a new recipe step and returns it.</summary>
    Task<RecipeStep> AddAsync(RecipeStep step, CancellationToken cancellationToken = default);

    /// <summary>Updates the given recipe step and returns the updated state.</summary>
    Task<RecipeStep> UpdateAsync(RecipeStep step, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes the recipe step with the given id.</summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
