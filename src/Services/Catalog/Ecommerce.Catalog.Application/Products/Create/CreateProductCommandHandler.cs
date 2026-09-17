using Ecommerce.Catalog.Application.Abstractions.Persistence;
using Ecommerce.Catalog.Application.Common.Results;
using Ecommerce.Catalog.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Ecommerce.Catalog.Application.Products.Create
{
    public sealed class CreateProductCommandHandler(
        ICatalogRepository repository,
        IValidator<CreateProductCommand> validator)
        : IRequestHandler<CreateProductCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var validation = await ValidateRequest(request, cancellationToken);
            if (!validation.IsSuccess) return validation;


            var categoryExists = await repository.CategoryExistsAsync(request.CategoryId, cancellationToken);

            if (!categoryExists) return Result<Guid>.Failure(new Error("Category.NotFound", "The category doesn't exist."));


            var product = new Product(
                request.Name,
                request.Price,
                request.Stock,
                request.CategoryId
            ); // I need to implement AutoMapper later to avoid create the objects manually.


            await repository.AddProductAsync(product, cancellationToken);

            await repository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(product.Id);

       
        }

        public async Task<Result<Guid>> ValidateRequest(CreateProductCommand request, CancellationToken cancellation)
        {
            var validationResult = await validator.ValidateAsync(request, cancellation);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(err => err.ErrorMessage);
                var message = string.Join("; ", errors);

                return Result<Guid>.Failure(
                    new Error("Product.Validation", message)
                );
            }   

            // Validation passed: return a success result marker (no product id yet).
            return Result<Guid>.Success(default);
        }
    }
}
