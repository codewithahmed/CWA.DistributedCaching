namespace DistributedCachingSampleApp.Settings;

public class CacheEntryOptions
{
    public int? AbsoluteExpirationInHours { get; set; }
    public int? SlidingExpirationInMinutes { get; set; }
}