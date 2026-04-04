using System.Text.Json;
using Kin.KinHub.KinHub.Domain;
using Kin.KinHub.KinHub.Domain.Common;

namespace Kin.KinHub.KinHub.Json;

public abstract class JsonRepository<TModel, TKey>
    where TModel : class, IEntity<TKey>
    where TKey : notnull
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    protected JsonRepository(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(_filePath);
        if (directory is not null && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    protected async Task<List<TModel>> ReadAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<TModel>>(json, _jsonOptions) ?? [];
        }
        finally
        {
            _lock.Release();
        }
    }

    protected async Task WriteAllAsync(List<TModel> items)
    {
        await _lock.WaitAsync();
        try
        {
            var json = JsonSerializer.Serialize(items, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<TModel> CreateAsync(TModel model)
    {
        var items = await ReadAllAsync();
        await OnBeforeCreateAsync(items, model);
        items.Add(model);
        await WriteAllAsync(items);
        return model;
    }

    public async Task<TModel> GetAsync(TKey key)
    {
        var items = await ReadAllAsync();
        var item = items.FirstOrDefault(x => x.Id.Equals(key));
        if (item is null)
            throw new EntityNotFoundException(typeof(TModel).Name, key);
        return item;
    }

    public async Task<TModel> UpdateAsync(TKey key, TModel model)
    {
        var items = await ReadAllAsync();
        var index = items.FindIndex(x => x.Id.Equals(key));
        if (index < 0)
            throw new EntityNotFoundException(typeof(TModel).Name, key);
        items[index] = model;
        await WriteAllAsync(items);
        return model;
    }

    public async Task<TModel> DeleteAsync(TKey key)
    {
        var items = await ReadAllAsync();
        var index = items.FindIndex(x => x.Id.Equals(key));
        if (index < 0)
            throw new EntityNotFoundException(typeof(TModel).Name, key);
        var removed = items[index];
        items.RemoveAt(index);
        await WriteAllAsync(items);
        return removed;
    }

    protected virtual Task OnBeforeCreateAsync(List<TModel> existingItems, TModel newItem) =>
        Task.CompletedTask;
}
