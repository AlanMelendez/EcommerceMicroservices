using Ecommerce.Catalog.Application.Abstractions.Caching;
using Ecommerce.Catalog.Application.Abstractions.Persistence;
using Ecommerce.Catalog.Infrastructure.Caching;
using Ecommerce.Catalog.Infrastructure.Caching.Configuration;
using Ecommerce.Catalog.Infrastructure.Persistence;
using Ecommerce.Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Ecommerce.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddCaching(configuration);

        return services;
    }

    private static void AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CatalogDatabase")
            ?? throw new InvalidOperationException(
                "Catalog database connection string was not found.");

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<ICatalogRepository, CatalogRepository>();
    }

    private static void AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                "Redis connection string is required.")
            .Validate(options => options.DefaultExpirationMinutes > 0,
                "Default expiration minutes must be greater than zero.")
            .ValidateOnStart();

        services.AddStackExchangeRedisCache(_ => { });
        services.AddOptions<RedisCacheOptions>()
            .Configure<IOptions<RedisOptions>>((cacheOptions, catalogOptions) =>
            {
                var redis = catalogOptions.Value;
                var connectionOptions = ConfigurationOptions.Parse(redis.ConnectionString);
                connectionOptions.AbortOnConnectFail = false;
                connectionOptions.ConnectRetry = 3;
                connectionOptions.ConnectTimeout = 5000;
                connectionOptions.SyncTimeout = 5000;

                cacheOptions.ConfigurationOptions = connectionOptions;
                cacheOptions.InstanceName = redis.InstanceName;
            });

        services.AddScoped<ICacheService, RedisCacheService>();
    }
}
