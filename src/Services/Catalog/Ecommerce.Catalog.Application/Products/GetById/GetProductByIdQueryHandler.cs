using Ecommerce.Catalog.Application.Abstractions.Caching;
using Ecommerce.Catalog.Application.Abstractions.Persistence;
using Ecommerce.Catalog.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Catalog.Application.Products.GetById
{
    public sealed class GetProductByIdQueryHandler(
        ICatalogRepository repository,
        ICacheService cacheService
        ) : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
    {
        public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {

            var cacheKey = $"catalog:product:{request.Id}";

            var cacheProduct = await cacheService.GetAsync<ProductResponse>(cacheKey, cancellationToken);

            if (cacheProduct is not null)
            {
                return Result<ProductResponse>.Success(cacheProduct);
            }



            var product =
            await repository.GetProductByIdAsync(
                request.Id,
                cancellationToken);

            if (product is null)
            {
                return Result<ProductResponse>.Failure(
                    new Error(
                        "Product.NotFound",
                        "The product was not found."));
            }

            var response = new ProductResponse(
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.CategoryId);

            await cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

            return Result<ProductResponse>.Success(response);
        }
    }
}
