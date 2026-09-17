using Ecommerce.Catalog.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Catalog.Application.Abstractions.Persistence
{
    public interface ICatalogRepository
    {
        Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellation);
        Task AddProductAsync(Product product, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
