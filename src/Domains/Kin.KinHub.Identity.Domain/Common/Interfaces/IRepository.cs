namespace Kin.KinHub.Identity.Domain.Common;

public interface IRepository<TModel, TKey>
where TModel : class
{
    Task<TModel> CreateAsync(TModel model);
    Task<TModel> DeleteAsync(TKey key);
    Task<TModel> UpdateAsync(TKey key, TModel model);
    Task<TModel> GetAsync(TKey key);
}
