using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mubank.Models;
using Mubank.Services;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RegisterConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DataContext _context;

        public RegisterConnectionMiddleware(RequestDelegate next, DataContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
           
            var Connect = new HostConnectLogModel()
            {
                ConnectID = Guid.NewGuid(),
                IpAddress = httpContext.Connection.LocalIpAddress.ToString(),
                Message = $"usuario do ip: {httpContext.Connection.LocalIpAddress.ToString()} se conectou a api na data: {DateTime.Now} utilizando um metodo do tipo: {httpContext.Request.Method} na rota: {httpContext.GetEndpoint().DisplayName}",
                Port = httpContext.Connection.LocalPort,
                date = DateTime.Now
            };

            await _context.HostConnectLog.AddAsync(Connect);

            await _context.SaveChangesAsync();

             _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RegisterConnectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseRegisterConnectionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RegisterConnectionMiddleware>();
        }
    }
}
