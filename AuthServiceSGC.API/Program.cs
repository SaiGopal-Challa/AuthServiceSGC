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

namespace AuthServiceSGC.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add Redis configuration
            var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
            builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
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
            builder.Services.AddScoped<IUserRepository, UserRepository>();

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
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
