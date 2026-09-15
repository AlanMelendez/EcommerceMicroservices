namespace Ecommerce.Catalog.Domain.Entities;

public sealed class Product
{
    private Product()
    {
    }

    public Product(
        string name,
        decimal price,
        int stock,
        Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Product name is required.",
                nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(price),
                "Product price cannot be negative.");
        }

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stock),
                "Product stock cannot be negative.");
        }

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Stock = stock;
        CategoryId = categoryId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public Guid CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public DateTime CreatedAt { get; private set; }
}