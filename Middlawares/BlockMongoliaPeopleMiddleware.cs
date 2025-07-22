using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mubank.Models;
using Mubank.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BlockMongoliaPeopleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _blockedIps = new();
        private readonly DataContext _dataContext;

        public BlockMongoliaPeopleMiddleware(RequestDelegate next, DataContext dataContext)
        {
            _next = next;
            _dataContext = dataContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var ip = context.Request.Headers.ContainsKey("X-Forwarded-For")
            ? context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0]
            : context.Connection.RemoteIpAddress?.ToString();

            var GeoModel = GetLocalizedModel(ip).Result;

            if (GeoModel.Country != "Mongolia")
            {
                context.Response.StatusCode = 403;

                context.Response.WriteAsync("Seu IP está bloqueado.\n motivo: Seu país é o mesmo do genshis kam, mongol!");

                return;
            }

            return _next(httpContext);
        }

        public async Task<DataIPLocalizedModel> GetLocalizedModel(string ipadress)
        {
            using var http = new HttpClient();
            var response = await http.GetAsync($"https://ipapi.co/{ipadress}/json/");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var geo = JsonSerializer.Deserialize<DataIPLocalizedModel>(json);

            return geo;
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BlockMongoliaPeopleMiddlewareExtensions
    {
        public static IApplicationBuilder UseBlockMongoliaPeopleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BlockMongoliaPeopleMiddleware>();
        }
    }
}
