using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;
using SMAIAXBackend.Infrastructure;
using SMAIAXBackend.Infrastructure.Repositories;

namespace SMAIAXBackend.API.ApplicationConfigurations;

[ExcludeFromCodeCoverage]
public static class RepositoryExtensions
{
    public static void AddRepositoryConfigurations(this IServiceCollection services)
    {
        services.AddScoped<IMeasurementRepository, MeasurementRepository>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<ISmartMeterRepository, SmartMeterRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
    }
}