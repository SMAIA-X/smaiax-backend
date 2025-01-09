using System.ComponentModel.DataAnnotations;

namespace SMAIAXBackend.Application.DTOs;

public class ContractCreateDto(Guid policyId)
{
    [Required] public Guid PolicyId { get; set; } = policyId;
}