using Ecommerce.Catalog.Application.Abstractions.Persistence;
using Ecommerce.Catalog.Domain.Entities;
using Ecommerce.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Catalog.Infrastructure.Repositories
{
    public sealed class CatalogRepository: ICatalogRepository
    {

        private readonly CatalogDbContext dbContext;

        private CatalogRepository()
        {
            //Used by EF Core when try to match the entities.
        }

        public CatalogRepository(CatalogDbContext _dbContext)
        {
            dbContext= _dbContext;
        }

        public async Task<Product?> GetProductByIdAsync(
       Guid id,
       CancellationToken cancellationToken)
        {
            return await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    product => product.Id == id,
                    cancellationToken);
        }

        public async Task<bool> CategoryExistsAsync(
            Guid categoryId,
            CancellationToken cancellationToken)
        {
            return await dbContext.Categories
                .AnyAsync(
                    category => category.Id == categoryId,
                    cancellationToken);
        }

        public async Task AddProductAsync(
            Product product,
            CancellationToken cancellationToken)
        {
            await dbContext.Products.AddAsync(
                product,
                cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            await dbContext.SaveChangesAsync(
                cancellationToken);
        }

    }
}
