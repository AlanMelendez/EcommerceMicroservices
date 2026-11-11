namespace Ecommerce.Catalog.Api.Contracts.Products
{
    public sealed record CreateProductRequest(
        string Name,
        decimal Price,
        int Stock,
        Guid CateogoryId
     );No
}
