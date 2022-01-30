namespace DistributedCachingSampleApp.CacheKey;
public static class CustomerCacheKeys
{
    public static string ListKey => "CustomerList";
    public static string GetKey(int customerId)
    {
        return $"Customer-{customerId}";
    }
}

