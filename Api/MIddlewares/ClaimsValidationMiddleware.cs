using Api.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Api.MIddlewares
{
    public class ClaimsValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var user = context.User;

            if (user.Identity.IsAuthenticated)
            {
                var dbContext = (bcareContext)context.RequestServices.GetService(typeof(bcareContext));
                var token = user.Claims.FirstOrDefault(e => e.Type == JwtRegisteredClaimNames.Jti);
                var cryptedUserToken = token.Value;

                var configurationProvider = (IConfiguration)context.RequestServices.GetService(typeof(IConfiguration));
                var dataProtectionProvider = (IDataProtectionProvider)context.RequestServices.GetService(typeof(IDataProtectionProvider));

                var key = configurationProvider["Authentication:jwt:key"];
                var protector = dataProtectionProvider.CreateProtector(key);
                var userToken = protector.Unprotect(cryptedUserToken);

                if (!dbContext.User.Any(e => e.Token == userToken))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("JWT Token Invalidated");

                    return;
                }
            }

            await _next(context);
        }
    }
}
