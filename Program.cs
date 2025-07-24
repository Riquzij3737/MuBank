
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mubank.Middlawares;
using Mubank.Models.DTOS;
using Mubank.Services;
using Mubank.Services.IServices;
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
                    ValidateAudience = false, // pode ser true se quiser validar audiência
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecrets:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            builder.Services.AddOpenApi();
            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MuBank API",
                    Version = "v1",
                    Description = "API bancária para gerenciamento de contas, usuários e transações.",
                    Contact = new OpenApiContact
                    {
                        Name = "Henrique Maurel",
                        Email = "henriquemaurel37@gmail.com"
                    }
                });

                // Autenticação JWT no Swagger
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no formato: Bearer {seu_token_aqui}"
                };

                options.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                };

                options.AddSecurityRequirement(securityRequirement);

            });

            builder.Services.AddScoped<ITokenService, TokenService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.UseRegisterConnectionMiddleware();
            app.UseSearchBlockIPsMiddleware();
            app.UseRateLimitMiddleware();
            app.UseErrorMiddleware();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();


            app.UseCors("AllowAll");

            app.Run();


        }
    }
}
