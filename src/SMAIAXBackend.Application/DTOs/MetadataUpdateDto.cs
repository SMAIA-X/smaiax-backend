using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Application.DTOs;

public class MetadataUpdateDto(Guid id, DateTime validFrom, LocationDto? location, int? householdSize)
{
    [Required] public Guid Id { get; set; } = id;

    [Required] public DateTime ValidFrom { get; set; } = validFrom;

    public LocationDto? Location { get; set; } = location;

    public int? HouseholdSize { get; set; } = householdSize;

    public static Metadata FromMetadataDto(MetadataUpdateDto metadataUpdateDto, SmartMeterId smartMeterId)
    {
        var location = metadataUpdateDto.Location != null
            ? new Location(metadataUpdateDto.Location.StreetName, metadataUpdateDto.Location.City,
                metadataUpdateDto.Location.State, metadataUpdateDto.Location.Country,
                metadataUpdateDto.Location.Continent)
            : null;
        return Metadata.Create(new MetadataId(metadataUpdateDto.Id), metadataUpdateDto.ValidFrom, location,
            metadataUpdateDto.HouseholdSize, smartMeterId);
    }
}