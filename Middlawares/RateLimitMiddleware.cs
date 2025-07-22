using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, int> RateLimits = new Dictionary<string, int>();
        private static int ConectionsLentgh;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            ConectionsLentgh++;

            foreach (var Rates in RateLimits)
            {
                if (Rates.Key == httpContext.Connection.RemoteIpAddress.ToString())
                {
                    if (Rates.Value >= 100)
                    {
                        httpContext.Response.StatusCode = 429; // Too Many Requests
                        return httpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    }
                    else
                    {
                        RateLimits[Rates.Key] = Rates.Value + 1;
                    }
                }
            }

            RateLimits.Add(httpContext.Connection.RemoteIpAddress.ToString(), ConectionsLentgh);
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimitMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }
    }
}
