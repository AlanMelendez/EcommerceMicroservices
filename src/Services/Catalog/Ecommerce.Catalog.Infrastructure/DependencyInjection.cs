using Ecommerce.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("CatalogDatabase")
            ?? throw new InvalidOperationException(
                "Catalog database connection string was not found.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}