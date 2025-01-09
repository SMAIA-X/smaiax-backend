using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Domain.Specifications;

public class PolicyLocationResolutionSpecification(LocationResolution locationResolution) : ISpecification<Policy>
{
    public bool IsSatisfiedBy(Policy policy)
    {
        return policy.LocationResolution <= locationResolution;
    }
}