using Kin.KinHub.Core.Domain;
using Kin.KinHub.Core.Domain.Interfaces.Recipes;
using Kin.KinHub.Core.Domain.Recipes;
using Kin.KinHub.Core.PostgreSql.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Core.PostgreSql.Repositories.Recipe;

public sealed class RecipeRepository : PostgreSqlRepository<RecipeEntity, Domain.Recipes.Recipe, Guid>, IRecipeRepository
{
    public RecipeRepository(CoreDbContext context)
        : base(context) { }

    /// <inheritdoc/>
    public async Task<Domain.Recipes.Recipe?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await Set
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
        return entity?.Adapt<Domain.Recipes.Recipe>();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Domain.Recipes.Recipe>> GetAllByFamilyIdAsync(Guid recipeBookId, CancellationToken cancellationToken = default)
    {
        var entities = await Set
            .Where(e => e.RecipeBookId == recipeBookId && !e.IsDeleted)
            .ToListAsync(cancellationToken);
        return entities.Adapt<IReadOnlyList<Domain.Recipes.Recipe>>();
    }

    /// <inheritdoc/>
    public async Task<Domain.Recipes.Recipe> AddAsync(Domain.Recipes.Recipe recipe, CancellationToken cancellationToken = default)
    {
        var entity = recipe.Adapt<RecipeEntity>();
        await Set.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity.Adapt<Domain.Recipes.Recipe>();
    }

    /// <inheritdoc/>
    public async Task<Domain.Recipes.Recipe> UpdateAsync(Domain.Recipes.Recipe recipe, CancellationToken cancellationToken = default)
    {
        var existing = await Set.FindAsync([recipe.Id], cancellationToken);
        if (existing is null)
            throw new EntityNotFoundException(nameof(RecipeEntity), recipe.Id);
        Context.Entry(existing).CurrentValues.SetValues(recipe.Adapt<RecipeEntity>());
        await Context.SaveChangesAsync(cancellationToken);
        return existing.Adapt<Domain.Recipes.Recipe>();
    }

    /// <inheritdoc/>
    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await Set.FindAsync([id], cancellationToken);
        if (existing is null)
            throw new EntityNotFoundException(nameof(RecipeEntity), id);
        existing.IsDeleted = true;
        await Context.SaveChangesAsync(cancellationToken);
    }
}
