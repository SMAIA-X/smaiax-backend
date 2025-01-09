using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.Contract;

public static class CreateContractEndpoint
{
    public static async Task<Ok<Guid>> Handle(IContractCreateService contractCreateService,
        [FromBody] ContractCreateDto contractCreateDto)
    {
        var createdContractId = await contractCreateService.CreateContractAsync(contractCreateDto);
        return TypedResults.Ok(createdContractId);
    }
}