namespace CWA.DistributedCache;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

public class CacheManager : ICacheManager
{
    private readonly IDistributedCache _distributedCache;

    public CacheManager(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public T? Get<T>(string cacheKey)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        byte[] objUtf8Bytes = _distributedCache.Get(cacheKey);
        
        return objUtf8Bytes is { } data
            ? JsonSerializer.Deserialize<T>(data)
            : default;
    }
    public async Task<T?> GetAsync<T>(string cacheKey, CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        byte[] objUtf8Bytes = await _distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);
        
        return objUtf8Bytes is { } data
            ? JsonSerializer.Deserialize<T>(data)
            : default;
    }
   
    public void Set<T>(string cacheKey, T obj, DistributedCacheEntryOptions options)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        
        byte[] objUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj); 
        _distributedCache.Set(cacheKey, objUtf8Bytes, options);
    }
    public async Task SetAsync<T>(
        string cacheKey, 
        T obj, 
        DistributedCacheEntryOptions options, 
        CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        
        byte[] objUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(obj);
        await _distributedCache.SetAsync(cacheKey, objUtf8Bytes, options, token).ConfigureAwait(false);
    }
    
    public void Refresh(string cacheKey)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        _distributedCache.Refresh(cacheKey);
    }

    public async Task RefreshAsync(string cacheKey, CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        await _distributedCache.RefreshAsync(cacheKey);
    }
   
    public Task Remove<T>(string cacheKey)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        _distributedCache.Remove(cacheKey);
         return Task.CompletedTask;
    }
    public async Task RemoveAsync<T>(string cacheKey, CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
       
        await _distributedCache.RemoveAsync(cacheKey, token).ConfigureAwait(false);
    }
    
    public void AddToList<T>(
        string cacheKey,
        T item,
        DistributedCacheEntryOptions options)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        byte[] utf8Bytes =  _distributedCache.Get(cacheKey);

        if (utf8Bytes != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

            if (itemList != null)
            {
                itemList.Add(item);

                utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(itemList);
                 _distributedCache.Set(cacheKey, utf8Bytes, options);
            }
        }
    }
    
    public void AddToList<T, TKey>(
        string cacheKey,
        T item,
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        if (orderBy == null)
        {
            throw new ArgumentNullException(nameof(orderBy));
        }

        byte[] utf8Bytes =  _distributedCache.Get(cacheKey);

        if (utf8Bytes != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

            if (itemList != null)
            {
                itemList.Add(item);
                
                itemList = isDesc 
                    ? itemList.OrderByDescending(orderBy).ToList() 
                    : itemList.OrderBy(orderBy).ToList();
                
                utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(itemList);
                 _distributedCache.Set(cacheKey, utf8Bytes, options);
            }
        }
    }
    
    public async Task AddToListAsync<T>(
        string cacheKey,
        T item,
        DistributedCacheEntryOptions options,
        CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        byte[] utf8Bytes = await _distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);

        if (utf8Bytes != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

            if (itemList != null)
            {
                itemList.Add(item);

                utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(itemList);
                await _distributedCache.SetAsync(cacheKey, utf8Bytes, options, token).ConfigureAwait(false);
            }
        }
    }
    
    
    public async Task AddToListAsync<T, TKey>(
        string cacheKey, 
        T item, 
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options, 
        CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        if (orderBy == null)
        {
            throw new ArgumentNullException(nameof(orderBy));
        }
        byte[] items = await _distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);

        if (items != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(items);

            if (itemList != null)
            {
                itemList.Add(item);
                
                itemList = isDesc 
                    ? itemList.OrderByDescending(orderBy).ToList() 
                    : itemList.OrderBy(orderBy).ToList();
                
                items = JsonSerializer.SerializeToUtf8Bytes(itemList);
                await _distributedCache.SetAsync(cacheKey, items, options, token).ConfigureAwait(false);
            }
        }
    }
    
    public void UpdateInList<T>(
        string cacheKey,
        Predicate<T> predicate,
        T updatedItem,
        DistributedCacheEntryOptions options)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (updatedItem == null)
        {
            throw new ArgumentNullException(nameof(updatedItem));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        byte[] utf8Bytes = _distributedCache.Get(cacheKey);

        if (utf8Bytes != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

            if (itemList != null)
            {
                int itemIndex = itemList.FindIndex(predicate);
                itemList[itemIndex] = updatedItem;

                utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(itemList); 
                _distributedCache.SetAsync(cacheKey, utf8Bytes, options);
            }
        }
    }
    
    public void UpdateInList<T, TKey>(
        string cacheKey,
        Predicate<T> predicate,
        T updatedItem,
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (updatedItem == null)
        {
            throw new ArgumentNullException(nameof(updatedItem));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        } 
        if (orderBy == null)
        {
            throw new ArgumentNullException(nameof(orderBy));
        }

        byte[] utf8Bytes = _distributedCache.Get(cacheKey);

        if (utf8Bytes != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

            if (itemList != null)
            {
                int itemIndex = itemList.FindIndex(predicate);
                itemList[itemIndex] = updatedItem;

                itemList = isDesc 
                    ? itemList.OrderByDescending(orderBy).ToList() 
                    : itemList.OrderBy(orderBy).ToList();
                
                utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(itemList); 
                _distributedCache.SetAsync(cacheKey, utf8Bytes, options);
            }
        }
    }
    
    public async Task UpdateInListAsync<T>(
        string cacheKey,
        Predicate<T> predicate,
        T updatedItem,
        DistributedCacheEntryOptions options,
        CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }

        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (updatedItem == null)
        {
            throw new ArgumentNullException(nameof(updatedItem));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        byte[] utf8Bytes = await _distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);

        if (utf8Bytes != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(utf8Bytes);

            if (itemList != null)
            {
                int itemIndex = itemList.FindIndex(predicate);
                itemList[itemIndex] = updatedItem;

                utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(itemList);
                await _distributedCache.SetAsync(cacheKey, utf8Bytes, options, token).ConfigureAwait(false);
            }
        }
    }

    public async Task UpdateInListAsync<T, TKey>(
        string cacheKey, 
        Predicate<T> predicate,
        T updatedItem, 
        Func<T, TKey> orderBy,
        bool isDesc,
        DistributedCacheEntryOptions options, 
        CancellationToken token = default)
    {
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        if (updatedItem == null)
        {
            throw new ArgumentNullException(nameof(updatedItem));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        } 
        if (orderBy == null)
        {
            throw new ArgumentNullException(nameof(orderBy));
        }
        
        byte[] items = await _distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);

        if (items != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(items);

            if (itemList != null)
            {
                int itemIndex = itemList.FindIndex(predicate);
                itemList[itemIndex] = updatedItem;
                
                itemList = isDesc 
                    ? itemList.OrderByDescending(orderBy).ToList() 
                    : itemList.OrderBy(orderBy).ToList();
                
                items = JsonSerializer.SerializeToUtf8Bytes(itemList);
                await _distributedCache.SetAsync(cacheKey, items, options, token).ConfigureAwait(false);
            }
        }
    }
    
    public void RemoveFromList<T>(
        string cacheKey, 
        Predicate<T?> predicate, 
        DistributedCacheEntryOptions options)
    { 
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        
        byte[] items =  _distributedCache.Get(cacheKey);

        if (items != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(items);

            if (itemList != null)
            {
                T? itemToBeRemoved = itemList.Find(predicate);

                if (itemToBeRemoved != null)
                {
                    itemList.Remove(itemToBeRemoved);

                    items = JsonSerializer.SerializeToUtf8Bytes(itemList); 
                    _distributedCache.Set(cacheKey, items, options);
                }
            }
        }
    }
    public async Task RemoveFromListAsync<T>(
        string cacheKey, 
        Predicate<T?> predicate, 
        DistributedCacheEntryOptions options, 
        CancellationToken token = default)
    { 
        if (cacheKey == null)
        {
            throw new ArgumentNullException(nameof(cacheKey));
        }
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        
        byte[] items = await _distributedCache.GetAsync(cacheKey, token).ConfigureAwait(false);

        if (items != null)
        {
            List<T>? itemList = JsonSerializer.Deserialize<List<T>>(items);

            if (itemList != null)
            {
                T? itemToBeRemoved = itemList.Find(predicate);

                if (itemToBeRemoved != null)
                {
                    itemList.Remove(itemToBeRemoved);

                    items = JsonSerializer.SerializeToUtf8Bytes(itemList);
                    await _distributedCache.SetAsync(cacheKey, items, options, token).ConfigureAwait(false);
                }
            }
        }
    }
    
}


