using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.Services.Interfaces;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class RemoveMetadataEndpoint
{
    public static async Task<NoContent> Handle(
        ISmartMeterDeleteService smartMeterDeleteService,
        [FromRoute] Guid smartMeterId,
        [FromRoute] Guid metadataId,
        ClaimsPrincipal user)
    {
        await smartMeterDeleteService.DeleteMetadataAsync(smartMeterId, metadataId);

        return TypedResults.NoContent();
    }
}