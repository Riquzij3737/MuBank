using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mubank.Models;
using Mubank.Services;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, List<DateTime>> _requests = new();
        private static readonly object _lock = new();

        private readonly int _limite = 100;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(1);

        public RateLimiterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {
            var ip = context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";

            lock (_lock)
            {
                if (!_requests.ContainsKey(ip))
                    _requests[ip] = new List<DateTime>();
                
                _requests[ip].RemoveAll(dt => dt < DateTime.UtcNow - _intervalo);

                if (_requests[ip].Count >= _limite)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers["Retry-After"] = "60"; 

                    dataContext.IPsBlocked.Add(new IPsBlockedModel()
                    {
                        Ip = ip,
                        DateBlocked = DateTime.UtcNow,
                        Id = Guid.NewGuid(),
                        Reason = "Rate limit exceeded"
                    });

                    dataContext.SaveChanges();
                    return;
                }

                _requests[ip].Add(DateTime.UtcNow);
            }

            await _next(context);
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimitMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>();
        }
    }
}
