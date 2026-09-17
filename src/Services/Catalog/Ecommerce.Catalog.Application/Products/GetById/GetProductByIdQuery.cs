using Ecommerce.Catalog.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Catalog.Application.Products.GetById
{
    public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductResponse>>;
}
