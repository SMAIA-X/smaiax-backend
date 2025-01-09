using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Repositories;

public interface IContractRepository
{
    ContractId NextIdentity();
    Task AddAsync(Contract contract);
}