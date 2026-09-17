

namespace Ecommerce.Catalog.Application.Products.GetById
{
    public sealed record ProductResponse(Guid Id,
    string Name,
    decimal Price,
    int Stock,
    Guid CategoryId);

}
