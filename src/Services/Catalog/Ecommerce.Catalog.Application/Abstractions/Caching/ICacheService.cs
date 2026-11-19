using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Catalog.Application.Abstractions.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);

        Task SetAsync<T>(
            string key,
            T value,
            TimeSpan expiration,
            CancellationToken cancellationToken
        );

        Task RemoveAsync(string key, CancellationToken cancellationToken);
        
    }
}