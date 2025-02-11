﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Enums;

namespace SMAIAXBackend.API.Endpoints.Measurement;

public static class GetMeasurementsEndpoint
{
    public static async Task<Ok<MeasurementListDto>> Handle(IMeasurementListService measurementListService,
        [FromQuery] Guid smartMeterId,
        [FromQuery] MeasurementResolution? measurementResolution,
        [FromQuery] DateTime? startAt, [FromQuery] DateTime? endAt)
    {
        var measurementList = await measurementListService.GetMeasurementsBySmartMeterAndResolutionAsync(smartMeterId,
            measurementResolution ?? MeasurementResolution.Raw, startAt, endAt);

        return TypedResults.Ok(measurementList);
    }
}