using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Domain.Specifications;

public class MeasurementResolutionSpecification(MeasurementResolution measurementResolution) : ISpecification<Policy>
{
    public bool IsSatisfiedBy(Policy policy)
    {
        return policy.MeasurementResolution <= measurementResolution;
    }
}