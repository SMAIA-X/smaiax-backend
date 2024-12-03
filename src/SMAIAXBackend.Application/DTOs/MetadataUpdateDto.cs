using System.ComponentModel.DataAnnotations;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Application.DTOs;

public class MetadataUpdateDto(Guid id, DateTime validFrom, LocationDto location, int householdSize)
{
    [Required]
    public Guid Id { get; set; } = id;

    [Required]
    public DateTime ValidFrom { get; set; } = validFrom;

    [Required]
    public LocationDto Location { get; set; } = location;

    [Required]
    public int HouseholdSize { get; set; } = householdSize;

    public static Metadata FromMetadataDto(MetadataUpdateDto metadataUpdateDto, SmartMeterId smartMeterId)
    {
        return Metadata.Create(new MetadataId(metadataUpdateDto.Id), metadataUpdateDto.ValidFrom,
            new Location(metadataUpdateDto.Location.StreetName, metadataUpdateDto.Location.City,
                metadataUpdateDto.Location.State, metadataUpdateDto.Location.Country,
                metadataUpdateDto.Location.Continent), metadataUpdateDto.HouseholdSize, smartMeterId);
    }
}