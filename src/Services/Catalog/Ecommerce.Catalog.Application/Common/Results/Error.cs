using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Catalog.Application.Common.Results
{
   public sealed record Error(string Code, string Message)
    {
        public static Error NotFound(string message) => new("NotFound", message);
        public static Error Validation(string message) => new("Validation", message);
        public static Error Unauthorized(string message) => new("Unauthorized", message);
        public static Error InternalServerError(string message) => new("InternalServerError", message);
    }
}
