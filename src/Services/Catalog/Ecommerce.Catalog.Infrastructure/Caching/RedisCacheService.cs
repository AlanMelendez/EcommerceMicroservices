using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ecommerce.Catalog.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Catalog.Infrastructure.Caching
{
    public sealed class RedisCacheService(IDistributedCache cache) : ICacheService
    {
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            var value = await cache.GetStringAsync(key,cancellationToken);

            if(value is null) return default; // Default:

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        {
            var serializedValue = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
              AbsoluteExpirationRelativeToNow = expiration  
            };

            await cache.SetStringAsync(
                key,
                serializedValue,
                options,
                cancellationToken
            );
        }
    }
}