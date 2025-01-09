using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.Application.Services.Interfaces;

public interface IContractCreateService
{
    Task<Guid> CreateContractAsync(ContractCreateDto contractCreateDto);
}