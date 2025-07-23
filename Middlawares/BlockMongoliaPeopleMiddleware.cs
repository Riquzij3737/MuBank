using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Mubank.Models;
using Mubank.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mubank.Middlawares
{
    
    public class BlockMongoliaPeopleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _blockedIps = new();
        private DataContext _dataContext;

        public BlockMongoliaPeopleMiddleware(RequestDelegate next)
        {
            _next = next;            
        }

        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {

            _dataContext = dataContext;

            var ip = context.Request.Headers.ContainsKey("X-Forwarded-For")
            ? context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0]
            : context.Connection.RemoteIpAddress?.ToString();

            var GeoModel = await GetLocalizedModel(ip);
            var Modelblocked = _dataContext.IPsBlocked.FirstOrDefault(x => x.Ip == ip);

            try
            {
                if (GeoModel == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Could not retrieve geolocation data.");
                    return;
                } else
                {
                    if (GeoModel.Country == "Mongolia")
                    {
                        context.Response.StatusCode = 403;

                        await context.Response.WriteAsync("Seu IP está bloqueado.\n motivo: Seu país é o mesmo do genshis kam, mongol!");

                        await _dataContext.IPsBlocked.AddAsync(new IPsBlockedModel()
                        {
                            Id = Guid.NewGuid(),
                            Ip = ip,
                            Reason = "Pq o pais do cara é a mongolia mano, pais do genshis kam",
                            DateBlocked = DateTime.Now
                        });

                        await _dataContext.SaveChangesAsync();


                        return;
                    }
                    else if (Modelblocked != null)
                    {
                        context.Response.StatusCode = 403;

                        await context.Response.WriteAsync("Seu IP está bloqueado.\n motivo: Seu país é o mesmo do genshis kam, mongol!");

                        return;
                    }
                }
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

                await _dataContext.Errors.AddAsync(error);
                await _dataContext.SaveChangesAsync();
                await context.Response.WriteAsJsonAsync(error);

                return;
            }

            await _next(context);
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
