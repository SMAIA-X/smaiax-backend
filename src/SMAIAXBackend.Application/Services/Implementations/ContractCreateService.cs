using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class ContractCreateService(
    IContractRepository contractRepository,
    IPolicyRepository policyRepository,
    ITenantRepository tenantRepository,
    ITenantContextService tenantContextService
) : IContractCreateService
{
    public async Task<Guid> CreateContractAsync(ContractCreateDto contractCreateDto)
    {
        var contractId = contractRepository.NextIdentity();

        var currentTenant = await tenantContextService.GetCurrentTenantAsync();
        var tenants = await tenantRepository.GetAllAsync();
        Policy? policy = null;
        foreach (var tenant in tenants.Where(t => !t.Equals(currentTenant)))
        {
            var policies = await policyRepository.GetPoliciesByTenantAsync(tenant);
            policy = policies.FirstOrDefault(p => p.Id.Id == contractCreateDto.PolicyId);
        }

        if (policy == null)
        {
            throw new PolicyNotFoundException(contractCreateDto.PolicyId);
        }

        var createdAt = DateTime.UtcNow;
        var contract = Contract.Create(contractId, createdAt, new PolicyId(contractCreateDto.PolicyId));
        await contractRepository.AddAsync(contract);

        return contractId.Id;
    }
}