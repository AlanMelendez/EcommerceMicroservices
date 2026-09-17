

using Ecommerce.Catalog.Application.Common.Results;
using MediatR;


namespace Ecommerce.Catalog.Application.Products.Create
{
	public sealed record CreateProductCommand(
		string Name,
		decimal Price,
		int Stock,
		Guid CategoryId) : IRequest<Result<Guid>>;
	
}
