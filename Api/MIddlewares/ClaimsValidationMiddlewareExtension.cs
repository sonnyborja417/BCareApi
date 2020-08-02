using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.MIddlewares
{
    public static class ClaimsValidationMiddlewareExtension
    {
        public static IApplicationBuilder UseClaimsValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClaimsValidationMiddleware>();
        }
    }
}
