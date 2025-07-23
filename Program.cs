
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mubank.Middlawares;
using Mubank.Services;
using Mubank.Services.IServices;
using Mubank.Models.DTOS;
using System.Text;


namespace Mubank
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            var builder = WebApplication.CreateBuilder(args);            

            // Add services to the container.           
            builder.Services.AddControllers();
            builder.Configuration.AddJsonFile("C:\\Visual Studio Projects\\_NetProjects\\C#\\Mubank\\appsettings.Development.json");
    
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin() // ou .WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            builder.Services.AddDbContext<DataContext>(x =>            
            {
                x.UseSqlServer("Data Source=HENRIQZIN\\SQLEXPRESS;Initial Catalog=Mubank;Integrated Security=True;Pooling=False;Encrypt=True;Trust Server Certificate=True");
            });
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = false,  // pode ser true se quiser validar emissor
                    ValidateAudience = false, // pode ser true se quiser validar audi�ncia
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecrets:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            builder.Services.AddOpenApi();
            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ITokenService, TokenService>();            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }            

            app.UseHttpsRedirection();

            app.UseAuthentication();            

            app.UseAuthorization();

            app.MapControllers();
            

            app.UseCors("AllowAll");            

            app.Run();

            
        }
    }
}
