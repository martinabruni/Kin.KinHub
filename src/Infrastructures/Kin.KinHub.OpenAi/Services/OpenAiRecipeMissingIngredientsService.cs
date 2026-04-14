using Kin.KinHub.Core.Domain.Interfaces.AI;
using Kin.KinHub.Core.Domain.Interfaces.Recipes;

namespace Kin.KinHub.OpenAi.Services;

internal sealed class OpenAiRecipeMissingIngredientsService : IRecipeMissingIngredientsService
{
    private const float SimilarityThreshold = 0.85f;

    private readonly IRecipeIngredientRepository _recipeIngredientRepository;
    private readonly IFridgeIngredientRepository _fridgeIngredientRepository;

    public OpenAiRecipeMissingIngredientsService(
        IRecipeIngredientRepository recipeIngredientRepository,
        IFridgeIngredientRepository fridgeIngredientRepository)
    {
        _recipeIngredientRepository = recipeIngredientRepository;
        _fridgeIngredientRepository = fridgeIngredientRepository;
    }

    public async Task<IReadOnlyList<string>> GetMissingIngredientsAsync(
        Guid recipeId,
        Guid fridgeId,
        CancellationToken cancellationToken = default)
    {
        var recipeIngredients = await _recipeIngredientRepository.GetAllByFamilyIdAsync(recipeId, cancellationToken);
        var fridgeIngredients = await _fridgeIngredientRepository.GetAllByFamilyIdAsync(fridgeId, cancellationToken);

        var missing = new List<string>();

        foreach (var recipeIngredient in recipeIngredients)
        {
            if (recipeIngredient.Embedding is null)
            {
                missing.Add(recipeIngredient.Name);
                continue;
            }

            var found = fridgeIngredients.Any(fi =>
                fi.Embedding is not null &&
                CosineSimilarity(recipeIngredient.Embedding, fi.Embedding) >= SimilarityThreshold);

            if (!found)
                missing.Add(recipeIngredient.Name);
        }

        return missing;
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        var dot = 0f;
        var magA = 0f;
        var magB = 0f;

        for (var i = 0; i < a.Length && i < b.Length; i++)
        {
            dot += a[i] * b[i];
            magA += a[i] * a[i];
            magB += b[i] * b[i];
        }

        return magA is 0f || magB is 0f ? 0f : dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
    }
}
