using Kin.KinHub.Identity.Domain.Exceptions;
using Kin.KinHub.Identity.Domain.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kin.KinHub.Identity.Sql;

public abstract class SqlRepository<TModel, TKey>
    where TModel : class
{
    protected DbContext Context { get; }
    protected DbSet<TModel> Set => Context.Set<TModel>();

    protected SqlRepository(DbContext context)
    {
        Context = context;
    }

    public async Task<TModel> CreateAsync(TModel model)
    {
        await OnBeforeCreateAsync(model);
        await Set.AddAsync(model);
        await Context.SaveChangesAsync();
        return model;
    }

    public async Task<TModel> GetAsync(TKey key)
    {
        var entity = await Set.FindAsync(key);
        if (entity is null)
            throw new EntityNotFoundException(typeof(TModel).Name, key!);
        return entity;
    }

    public async Task<TModel> UpdateAsync(TKey key, TModel model)
    {
        var existing = await Set.FindAsync(key);
        if (existing is null)
            throw new EntityNotFoundException(typeof(TModel).Name, key!);
        Context.Entry(existing).CurrentValues.SetValues(model);
        await Context.SaveChangesAsync();
        return model;
    }

    public async Task<TModel> DeleteAsync(TKey key)
    {
        var existing = await Set.FindAsync(key);
        if (existing is null)
            throw new EntityNotFoundException(typeof(TModel).Name, key!);
        Set.Remove(existing);
        await Context.SaveChangesAsync();
        return existing;
    }

    protected virtual Task OnBeforeCreateAsync(TModel model) => Task.CompletedTask;
}
