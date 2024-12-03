namespace SMAIAXBackend.Application.Services.Interfaces;

public interface ISmartMeterDeleteService
{
    Task DeleteMetadataAsync(Guid smartMeterId, Guid metadataId);
}