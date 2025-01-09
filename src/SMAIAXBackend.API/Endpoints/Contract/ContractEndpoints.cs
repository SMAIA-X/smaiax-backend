using SMAIAXBackend.Application.DTOs;

namespace SMAIAXBackend.API.Endpoints.Contract;

public static class ContractEndpoints
{
    public static WebApplication MapContractEndpoints(this WebApplication app)
    {
        const string contentType = "application/json";
        var group = app.MapGroup("/api/contracts")
            .WithTags("Contract")
            .RequireAuthorization();

        group.MapPost("/", CreateContractEndpoint.Handle)
            .WithName("createContract")
            .Accepts<ContractCreateDto>(contentType)
            .Produces<Guid>(StatusCodes.Status200OK, contentType)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }
}