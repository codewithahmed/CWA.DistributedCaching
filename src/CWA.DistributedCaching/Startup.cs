using CWA.DistributedCache.Settings;

namespace CWA.DistributedCache;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class Startup
{
    public static void AddDistributedCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();

        if (settings.PreferRedis)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.RedisHost;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddTransient<ICacheManager, CacheManager>();
    }
}