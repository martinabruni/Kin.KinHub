namespace Kin.KinHub.Core.Domain.Interfaces.AI;

/// <summary>
/// Service for determining which recipe ingredients are missing from a fridge.
/// </summary>
public interface IRecipeMissingIngredientsService
{
    /// <summary>Returns the list of fridge ingredients missing to complete the recipe.</summary>
    Task<IReadOnlyList<string>> GetMissingIngredientsAsync(
        Guid recipeId,
        Guid fridgeId,
        CancellationToken cancellationToken = default);
}
