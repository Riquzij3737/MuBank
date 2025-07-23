using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mubank.Models;
using Mubank.Services;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private IMapper _mapper;
        private DataContext _context;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
            
        }

        public Task Invoke(HttpContext httpContext, DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

            try
            {
                return _next(httpContext);
            }
            catch (Exception ex)
            {
                var error = new ErrorModel()
                {
                    IdError = Guid.NewGuid(),
                    MessageError = ex.Message,
                    HttpStatusCode = 500,
                    Date = DateTime.Now
                };
                
                httpContext.Response.WriteAsJsonAsync(error);
                throw;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
