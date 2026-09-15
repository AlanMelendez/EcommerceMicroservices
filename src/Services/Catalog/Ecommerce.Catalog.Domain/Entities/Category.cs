namespace Ecommerce.Catalog.Domain.Entities;

public sealed class Category
{
    private Category()
    {
        //EF Core needs a way to create the object when it reads database records.
    }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Category name is required.",
                nameof(name));
        }

        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public ICollection<Product> Products { get; private set; }
        = new List<Product>();
}