using Microsoft.Extensions.Caching.Distributed;

namespace CWA.DistributedCache;

public interface ICacheManager
{
    T? Get<T>(string cacheKey);
    Task<T?> GetAsync<T>(string cacheKey, CancellationToken token = default);
    void Set<T>(string cacheKey, T obj, DistributedCacheEntryOptions options);

    Task SetAsync<T>(
        string cacheKey, 
        T obj, 
        DistributedCacheEntryOptions options, 
        CancellationToken token = default);

    void Refresh(string cacheKey);
    Task RefreshAsync(string cacheKey, CancellationToken token = default);
    Task Remove<T>(string cacheKey);
    Task RemoveAsync<T>(string cacheKey, CancellationToken token = default);

    void AddToList<T>(
        string cacheKey,
        T item,
        DistributedCacheEntryOptions options);  
    
    void AddToList<T, TKey>(
        string cacheKey,
        T item,
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options);

    Task AddToListAsync<T>(
        string cacheKey,
        T item,
        DistributedCacheEntryOptions options,
        CancellationToken token = default);

    Task AddToListAsync<T, TKey>(
        string cacheKey, 
        T item, 
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options, 
        CancellationToken token = default);

    void UpdateInList<T>(
        string cacheKey,
        Predicate<T> predicate,
        T updatedItem,
        DistributedCacheEntryOptions options);
    
    void UpdateInList<T, TKey>(
        string cacheKey,
        Predicate<T> predicate,
        T updatedItem,
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options);

    Task UpdateInListAsync<T>(
        string cacheKey,
        Predicate<T> predicate,
        T updatedItem,
        DistributedCacheEntryOptions options,
        CancellationToken token = default);

    Task UpdateInListAsync<T, TKey>(
        string cacheKey, 
        Predicate<T> predicate,
        T updatedItem,
        Func<T, TKey> orderBy,
        bool isDesc, 
        DistributedCacheEntryOptions options, 
        CancellationToken token = default);

    void RemoveFromList<T>(
        string cacheKey, 
        Predicate<T?> predicate, 
        DistributedCacheEntryOptions options);

    Task RemoveFromListAsync<T>(
        string cacheKey, 
        Predicate<T?> predicate, 
        DistributedCacheEntryOptions options, 
        CancellationToken token = default);
}