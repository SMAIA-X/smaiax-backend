using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.SmartMeter;

public static class SmartMeterEndpoints
{
    public static WebApplication MapSmartMeterEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/smartMeters")
            .WithTags("SmartMeter")
            .RequireAuthorization();

        group.MapGet("/", GetSmartMetersEndpoint.Handle)
            .WithName("getSmartMeters")
            .Produces<List<SmartMeterOverviewDto>>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", GetSmartMeterByIdEndpoint.Handle)
            .WithName("getSmartMeterById")
            .Produces<SmartMeterDto>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", AddSmartMeterEndpoint.Handle)
            .WithName("addSmartMeter")
            .Accepts<SmartMeterCreateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status201Created, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:guid}", UpdateSmartMeterEndpoint.Handle)
            .WithName("updateSmartMeter")
            .Accepts<SmartMeterUpdateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:guid}/metadata", AddMetadataEndpoint.Handle)
            .WithName("addMetadata")
            .Accepts<MetadataCreateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{smartMeterId:guid}/metadata/{metadataId:guid}", RemoveMetadataEndpoint.Handle)
            .WithName("removeMetadata")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{smartMeterId:guid}/metadata/{metadataId:guid}", UpdateMetadataEndpoint.Handle)
            .WithName("updateMetadata")
            .Accepts<MetadataUpdateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}