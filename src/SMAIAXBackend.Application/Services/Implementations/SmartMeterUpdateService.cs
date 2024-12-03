using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterUpdateService(
    ISmartMeterRepository smartMeterRepository,
    IPolicyRepository policyRepository,
    ILogger<SmartMeterUpdateService> logger) : ISmartMeterUpdateService
{
    public async Task<Guid> UpdateSmartMeterAsync(
        Guid smartMeterIdExpected,
        SmartMeterUpdateDto smartMeterUpdateDto)
    {
        if (smartMeterIdExpected != smartMeterUpdateDto.Id)
        {
            logger.LogWarning(
                "SmartMeterId `{SmartMeterIdExpected}` in the path does not match the SmartMeterId `{SmartMeterIdActual}` in the body",
                smartMeterIdExpected, smartMeterUpdateDto.Id);
            throw new SmartMeterIdMismatchException(smartMeterIdExpected, smartMeterUpdateDto.Id);
        }

        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterIdExpected);
            throw new SmartMeterNotFoundException(smartMeterIdExpected);
        }

        smartMeter.Update(smartMeterUpdateDto.Name);
        await smartMeterRepository.UpdateAsync(smartMeter);

        return smartMeterIdExpected;
    }

    public async Task<Guid> UpdateMetadataAsync(Guid smartMeterId, Guid metadataId, MetadataUpdateDto metadataUpdateDto)
    {
        if (metadataId != metadataUpdateDto.Id)
        {
            logger.LogWarning(
                "MetadataId `{MetadataIdExpected}` in the path does not match the MetadataId `{MetadataIdActual}` in the body",
                metadataId, metadataUpdateDto.Id);
            throw new MetadataIdMismatchException(metadataId, metadataUpdateDto.Id);
        }

        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterId);
            throw new SmartMeterNotFoundException(smartMeterId);
        }

        var policies = await policyRepository.GetPoliciesBySmartMeterIdAsync(smartMeter.Id);

        if (policies.Count != 0)
        {
            logger.LogWarning(
                "Cannot update metadata because there are existing policies for smart meter with id  {SmartMeterId}",
                smartMeterId);
            throw new ExistingPoliciesException(smartMeterId);
        }

        var metadata = MetadataUpdateDto.FromMetadataDto(metadataUpdateDto, smartMeter.Id);

        try
        {
            smartMeter.UpdateMetadata(metadata);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Metadata with id '{MetadataId}' not found", metadataId);
            throw new MetadataNotFoundException(metadataId);
        }

        await smartMeterRepository.UpdateAsync(smartMeter);

        return metadataId;
    }
}