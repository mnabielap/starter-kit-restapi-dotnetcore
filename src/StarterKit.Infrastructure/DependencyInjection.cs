using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StarterKit.Application.Common.Interfaces;
using StarterKit.Application.Contracts;
using StarterKit.Infrastructure.Data;
using StarterKit.Infrastructure.Data.Interceptors;
using StarterKit.Infrastructure.Repositories;
using StarterKit.Infrastructure.Services;

namespace StarterKit.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        // Database Configuration (Supports SQLite and PostgreSQL)
        var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "Sqlite";
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.AddInterceptors(interceptor);

            if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase))
            {
                // Requires: Npgsql.EntityFrameworkCore.PostgreSQL
                options.UseNpgsql(connectionString); 
            }
            else
            {
                // Default to SQLite
                options.UseSqlite(connectionString);
            }
        });

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IJwtService, JwtService>();

        return services;
    }
}