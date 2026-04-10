namespace Kin.KinHub.Core.Domain;

/// <summary>
/// Generic CRUD repository contract for domain models.
/// </summary>
/// <typeparam name="TModel">The domain model type.</typeparam>
/// <typeparam name="TKey">The type of the model's primary key.</typeparam>
public interface IRepository<TModel, TKey>
    where TModel : class
{
    /// <summary>
    /// Creates a new entity and returns it.
    /// </summary>
    Task<TModel> CreateAsync(TModel model);

    /// <summary>
    /// Returns the entity matching the given key, or throws if not found.
    /// </summary>
    Task<TModel> GetAsync(TKey key);

    /// <summary>
    /// Returns all entities.
    /// </summary>
    Task<IReadOnlyList<TModel>> GetAllAsync();

    /// <summary>
    /// Replaces the entity identified by key and returns the updated entity.
    /// </summary>
    Task<TModel> UpdateAsync(TKey key, TModel model);

    /// <summary>
    /// Deletes the entity identified by key and returns it.
    /// </summary>
    Task<TModel> DeleteAsync(TKey key);
}
