using Kin.KinHub.Core.Business.Common;
using Mapster;

namespace Kin.KinHub.Core.Business.RecipeAssistantFeature;

public sealed class KinHubRecipeAiService : IRecipeAiService
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IFridgeRepository _fridgeRepository;
    private readonly IFridgeIngredientRepository _fridgeIngredientRepository;
    private readonly IRecipeBookRepository _recipeBookRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeIngredientRepository _recipeIngredientRepository;
    private readonly IRecipeStepRepository _recipeStepRepository;
    private readonly IRecipeAssistantService _recipeAssistantService;

    public KinHubRecipeAiService(
        IFamilyRepository familyRepository,
        IFridgeRepository fridgeRepository,
        IFridgeIngredientRepository fridgeIngredientRepository,
        IRecipeBookRepository recipeBookRepository,
        IRecipeRepository recipeRepository,
        IRecipeIngredientRepository recipeIngredientRepository,
        IRecipeStepRepository recipeStepRepository,
        IRecipeAssistantService recipeAssistantService)
    {
        _familyRepository = familyRepository;
        _fridgeRepository = fridgeRepository;
        _fridgeIngredientRepository = fridgeIngredientRepository;
        _recipeBookRepository = recipeBookRepository;
        _recipeRepository = recipeRepository;
        _recipeIngredientRepository = recipeIngredientRepository;
        _recipeStepRepository = recipeStepRepository;
        _recipeAssistantService = recipeAssistantService;
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<RecipeSuggestionResponse>>> SuggestRecipesAsync(
        Guid fridgeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<IReadOnlyList<RecipeSuggestionResponse>>.NotFound("Family not found for the current user.");

        var fridge = await _fridgeRepository.GetByIdAsync(fridgeId, cancellationToken);
        if (fridge is null)
            return Result<IReadOnlyList<RecipeSuggestionResponse>>.NotFound("Fridge not found.");
        if (fridge.FamilyId != family.Id)
            return Result<IReadOnlyList<RecipeSuggestionResponse>>.Unauthorized("Access denied.");

        var fridgeIngredients = await _fridgeIngredientRepository.GetAllByFamilyIdAsync(fridgeId, cancellationToken);
        var fridgeAi = fridgeIngredients
            .Select(i => new RecipeIngredient { Id = Guid.Empty, Name = i.Name, Quantity = i.Quantity, MeasureUnit = i.MeasureUnit, RecipeId = Guid.Empty })
            .ToList();

        var books = await _recipeBookRepository.GetAllByFamilyIdAsync(family.Id, cancellationToken);
        var familyRecipes = new List<Recipe>();

        foreach (var book in books)
        {
            var recipes = await _recipeRepository.GetAllByFamilyIdAsync(book.Id, cancellationToken);
            foreach (var recipe in recipes)
            {
                var aiRecipe = await BuildAiRecipeAsync(recipe, cancellationToken);
                familyRecipes.Add(aiRecipe);
            }
        }

        var suggestions = await _recipeAssistantService.SuggestRecipesAsync(fridgeAi, familyRecipes, cancellationToken);
        return Result<IReadOnlyList<RecipeSuggestionResponse>>.Success(suggestions.Adapt<IReadOnlyList<RecipeSuggestionResponse>>());
    }

    /// <inheritdoc/>
    public async Task<Result<ParsedRecipeResponse?>> ParseRecipeAsync(
        string rawText,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeAssistantService.ParseRecipeAsync(rawText, cancellationToken);
        return Result<ParsedRecipeResponse?>.Success(recipe?.Adapt<ParsedRecipeResponse>());
    }

    /// <inheritdoc/>
    public async Task<Result<RecipeAdaptationResponse>> AdaptRecipeAsync(
        Guid recipeId,
        IReadOnlyList<string> constraints,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeAdaptationResponse>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null)
            return Result<RecipeAdaptationResponse>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null)
            return Result<RecipeAdaptationResponse>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<RecipeAdaptationResponse>.Unauthorized("Access denied.");

        var aiRecipe = await BuildAiRecipeAsync(recipe, cancellationToken);
        var result = await _recipeAssistantService.AdaptRecipeAsync(aiRecipe, constraints, cancellationToken);
        return Result<RecipeAdaptationResponse>.Success(result.Adapt<RecipeAdaptationResponse>());
    }

    private async Task<Recipe> BuildAiRecipeAsync(
        Recipe recipe,
        CancellationToken cancellationToken)
    {
        recipe.Ingredients = await _recipeIngredientRepository.GetAllByFamilyIdAsync(recipe.Id, cancellationToken);
        recipe.Steps = (await _recipeStepRepository.GetAllByFamilyIdAsync(recipe.Id, cancellationToken))
            .OrderBy(s => s.Order)
            .ToList();
        return recipe;
    }
}
