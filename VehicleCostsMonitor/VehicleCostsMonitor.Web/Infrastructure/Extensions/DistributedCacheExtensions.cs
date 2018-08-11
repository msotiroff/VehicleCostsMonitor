namespace VehicleCostsMonitor.Web.Infrastructure.Extensions
{
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public static class DistributedCacheExtensions
    {
        public static async Task<IDistributedCache> SetSerializableObject(this IDistributedCache cache, string key, object value, TimeSpan expiration)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var serializedObject = JsonConvert.SerializeObject(value, jsonSettings);

            await cache.SetStringAsync(key, serializedObject, cacheOptions);

            return cache;
        }
    }
}
