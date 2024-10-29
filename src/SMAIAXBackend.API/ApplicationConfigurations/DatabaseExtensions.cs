using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.API.ApplicationConfigurations;

[ExcludeFromCodeCoverage]
public static class DatabaseExtensions
{
    public static void AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("smaiax-db"));
        });
    }
}