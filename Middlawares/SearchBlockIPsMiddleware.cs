using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mubank.Services;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class SearchBlockIPsMiddleware
    {
        private readonly RequestDelegate _next;        

        public SearchBlockIPsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, DataContext context)
        {
            var  ip = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

            var block = context.IPsBlocked.
                Where(x => x.Ip == ip)
                .Any();

            if (block)
            {
                httpContext.Response.StatusCode = 403; // Forbidden
                httpContext.Response.WriteAsJsonAsync(new
                {
                    Message = "Your IP address is blocked from accessing this resource.",
                    StatusCode = 403
                });

                return _next(httpContext);
            }


                httpContext.Response.StatusCode = 200; // OK

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SearchBlockIPsMiddlewareExtensions
    {
        public static IApplicationBuilder UseSearchBlockIPsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SearchBlockIPsMiddleware>();
        }
    }
}
