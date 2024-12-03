using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class UpdateMetadataEndpoint
{
    public static async Task<Ok<Guid>> Handle(
        ISmartMeterUpdateService smartMeterUpdateService,
        [FromRoute] Guid smartMeterId,
        [FromRoute] Guid metadataId,
        [FromBody] MetadataUpdateDto metadataUpdateDto)
    {
        var updatedMetadataId =
            await smartMeterUpdateService.UpdateMetadataAsync(smartMeterId, metadataId, metadataUpdateDto);

        return TypedResults.Ok(updatedMetadataId);
    }
}