using AuthServiceSGC.Application.Interfaces;
using AuthServiceSGC.Application.Services;
using AuthServiceSGC.Infrastructure.Repositories;
using AuthServiceSGC.Infrastructure.Database;
using AuthServiceSGC.Infrastructure.Cache;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using AuthServiceSGC.Infrastructure.Services;
using AuthServiceSGC.Domain.Constants;

namespace AuthServiceSGC.API
{
    public class Program
    {
        // custom ratelimiting middleware is yet to be configured in builder and app
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Enable Razor Pages
            builder.Services.AddRazorPages();

            builder.Services.AddSession();

            // Add Redis configuration
            var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

            var jwtSecretKey = builder.Configuration.GetValue<string>("JwtSecretKey");
            AppsettingData.JWTSecretKey = jwtSecretKey;

            //builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                try
                {
                    var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection") + ",abortConnect=false";
                    return ConnectionMultiplexer.Connect(redisConnectionString);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    return null; // or handle with a Redis connection stub
                }
            });

            builder.Services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();


            // Add Oracle Database configuration
            var oracleConnectionString = builder.Configuration.GetConnectionString("OracleConnection");
            builder.Services.AddDbContext<OracleDbContext>(options =>
                options.UseOracle(oracleConnectionString));

            // Add PostgreSQL Database configuration
            var postgresConnectionString = builder.Configuration.GetConnectionString("PostgresConnection");
            builder.Services.AddDbContext<PostgreSqlDbContext>(options =>
                options.UseNpgsql(postgresConnectionString));

            // Add application services and repositories
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IOTPService, OTPService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITokenService, Application.Services.TokenService>();
            builder.Services.AddScoped<ISessionDetailsRepository, SessionDetailsRepository>();
            builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();



            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages();
            app.UseSession();
            //if (app.Environment.IsEnvironment("UI"))
            //{
            //    app.MapRazorPages();  // Only map Razor Pages for UI profile
            //}

            ////if (app.Environment.IsEnvironment("API") || app.Environment.IsProduction())
            //else
            //{
            //    app.MapControllers();  // Map API controllers for API or Production profile
            //}

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});

            app.Run();
        }
    }
}
