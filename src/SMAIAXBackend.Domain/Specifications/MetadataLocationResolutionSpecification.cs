using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.Domain.Specifications;

public class MetadataLocationResolutionSpecification(LocationResolution locationResolution) : ISpecification<Metadata>
{
    public bool IsSatisfiedBy(Metadata metadata)
    {
        if (locationResolution == LocationResolution.None)
        {
            return true;
        }

        if (metadata.Location == null)
        {
            return false;
        }

        return metadata.Location.GetLocationResolution() <= locationResolution;
    }
}