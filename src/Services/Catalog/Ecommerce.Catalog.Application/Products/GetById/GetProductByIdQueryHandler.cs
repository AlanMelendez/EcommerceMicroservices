using Ecommerce.Catalog.Application.Abstractions.Persistence;
using Ecommerce.Catalog.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Catalog.Application.Products.GetById
{
    public sealed class GetProductByIdQueryHandler(
        ICatalogRepository repository
        ) : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
    {
        public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
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

            return Result<ProductResponse>.Success(response);
        }
    }
}
