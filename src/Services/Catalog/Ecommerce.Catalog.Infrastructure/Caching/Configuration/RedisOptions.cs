using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Catalog.Infrastructure.Caching.Configuration
{
    public sealed class RedisOptions
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; set; } = string.Empty;
        public string InstanceName { get; set; } = "Catalog:";
        public int DefaultExpirationMinutes { get; set; } = 30;
    }
}