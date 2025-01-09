using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

using Npgsql;

using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Infrastructure.Configurations;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.API.ApplicationConfigurations;

[ExcludeFromCodeCoverage]
public static class DatabaseExtensions
{
    public static void AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfiguration>(configuration.GetSection("DatabaseConfiguration"));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbConfig = configuration.GetSection("DatabaseConfiguration").Get<DatabaseConfiguration>();
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = dbConfig!.Host,
                Port = dbConfig.Port,
                Username = dbConfig.SuperUsername,
                Password = dbConfig.SuperUserPassword,
                Database = dbConfig.MainDatabase
            };
            options.UseNpgsql(connectionStringBuilder.ConnectionString);
        });

        // To generate migrations use a hardcoded connection string and comment out 
        // the variables
        services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
        services.AddDbContext<TenantDbContext>((serviceProvider, options) =>
        {
            var tenantContextService = serviceProvider.GetRequiredService<ITenantContextService>();
            var tenantDbContextFactory = serviceProvider.GetRequiredService<ITenantDbContextFactory>();
            var currentTenant = tenantContextService.GetCurrentTenantAsync().GetAwaiter().GetResult();
            var connectionString = tenantDbContextFactory.GetConnectionStringForTenant(currentTenant).GetAwaiter()
                .GetResult();
            options.UseNpgsql(connectionString);

            // options.UseNpgsql("Host=localhost:5432;Username=user;Password=password;Database=tenant-template-db");
        });
    }
}