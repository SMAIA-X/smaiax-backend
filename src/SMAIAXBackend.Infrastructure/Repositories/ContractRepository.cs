using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Infrastructure.DbContexts;

namespace SMAIAXBackend.Infrastructure.Repositories;

public class ContractRepository(TenantDbContext tenantDbContext) : IContractRepository
{
    public ContractId NextIdentity()
    {
        return new ContractId(Guid.NewGuid());
    }

    public async Task AddAsync(Contract contract)
    {
        await tenantDbContext.Contracts.AddAsync(contract);
        await tenantDbContext.SaveChangesAsync();
    }
}