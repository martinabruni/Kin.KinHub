using Kin.KinHub.Core.Business.Common;

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
    public async Task<Result<IReadOnlyList<RecipeSuggestion>>> SuggestRecipesAsync(
        Guid fridgeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<IReadOnlyList<RecipeSuggestion>>.NotFound("Family not found for the current user.");

        var fridge = await _fridgeRepository.GetByIdAsync(fridgeId, cancellationToken);
        if (fridge is null)
            return Result<IReadOnlyList<RecipeSuggestion>>.NotFound("Fridge not found.");
        if (fridge.FamilyId != family.Id)
            return Result<IReadOnlyList<RecipeSuggestion>>.Unauthorized("Access denied.");

        var fridgeIngredients = await _fridgeIngredientRepository.GetAllByFamilyIdAsync(fridgeId, cancellationToken);
        var fridgeAi = fridgeIngredients
            .Select(i => new RecipeAssistantIngredient { Name = i.Name, Quantity = i.Quantity, Unit = i.MeasureUnit })
            .ToList();

        var books = await _recipeBookRepository.GetAllByFamilyIdAsync(family.Id, cancellationToken);
        var familyRecipes = new List<RecipeAssistantRecipe>();

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
        return Result<IReadOnlyList<RecipeSuggestion>>.Success(suggestions);
    }

    /// <inheritdoc/>
    public async Task<Result<RecipeAssistantRecipe?>> ParseRecipeAsync(
        string rawText,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeAssistantService.ParseRecipeAsync(rawText, cancellationToken);
        return Result<RecipeAssistantRecipe?>.Success(recipe);
    }

    /// <inheritdoc/>
    public async Task<Result<RecipeAdaptationResult>> AdaptRecipeAsync(
        Guid recipeId,
        IReadOnlyList<string> constraints,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var family = await _familyRepository.FindByUserIdAsync(userId, cancellationToken);
        if (family is null)
            return Result<RecipeAdaptationResult>.NotFound("Family not found for the current user.");

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null)
            return Result<RecipeAdaptationResult>.NotFound("Recipe not found.");

        var book = await _recipeBookRepository.GetByIdAsync(recipe.RecipeBookId, cancellationToken);
        if (book is null)
            return Result<RecipeAdaptationResult>.NotFound("Recipe book not found.");
        if (book.FamilyId != family.Id)
            return Result<RecipeAdaptationResult>.Unauthorized("Access denied.");

        var aiRecipe = await BuildAiRecipeAsync(recipe, cancellationToken);
        var result = await _recipeAssistantService.AdaptRecipeAsync(aiRecipe, constraints, cancellationToken);
        return Result<RecipeAdaptationResult>.Success(result);
    }

    private async Task<RecipeAssistantRecipe> BuildAiRecipeAsync(
        Recipe recipe,
        CancellationToken cancellationToken)
    {
        var ingredients = await _recipeIngredientRepository.GetAllByFamilyIdAsync(recipe.Id, cancellationToken);
        var steps = await _recipeStepRepository.GetAllByFamilyIdAsync(recipe.Id, cancellationToken);

        return new RecipeAssistantRecipe
        {
            Name = recipe.Name,
            Backstory = recipe.Backstory,
            FinalTime = recipe.FinalTime,
            Portions = recipe.Portions,
            Ingredients = ingredients
                .Select(i => new RecipeAssistantIngredient { Name = i.Name, Quantity = i.Quantity, Unit = i.MeasureUnit })
                .ToList(),
            Steps = steps
                .OrderBy(s => s.Order)
                .Select(s => new RecipeAssistantStep { Order = s.Order, Description = s.Description })
                .ToList(),
        };
    }
}
