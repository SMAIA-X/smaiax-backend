using Microsoft.Extensions.Logging;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;

namespace SMAIAXBackend.Application.Services.Implementations;

public class SmartMeterCreateService(
    ISmartMeterRepository smartMeterRepository,
    IMqttBrokerRepository mqttBrokerRepository,
    IVaultRepository vaultRepository,
    ITransactionManager transactionManager,
    ILogger<SmartMeterCreateService> logger) : ISmartMeterCreateService
{
    public async Task<Guid> AddSmartMeterAsync(SmartMeterCreateDto smartMeterCreateDto)
    {
        var smartMeterId = smartMeterRepository.NextIdentity();

        var metadataList = new List<Metadata>();
        if (smartMeterCreateDto.Metadata != null)
        {
            var metadataId = smartMeterRepository.NextMetadataIdentity();
            var locationDto = smartMeterCreateDto.Metadata.Location;
            var location = locationDto != null
                ? new Location(locationDto.StreetName,
                    locationDto.City,
                    locationDto.State, locationDto.Country,
                    locationDto.Continent)
                : null;
            var metadata = Metadata.Create(metadataId, smartMeterCreateDto.Metadata.ValidFrom, location,
                smartMeterCreateDto.Metadata.HouseholdSize, smartMeterId);

            metadataList.Add(metadata);
        }

        if (String.IsNullOrEmpty(smartMeterCreateDto.Name))
        {
            logger.LogError("Smart meter name is required.");
            throw new SmartMeterNameRequiredException();
        }

        await transactionManager.ReadCommittedTransactionScope(async () =>
        {
            var smartMeter = SmartMeter.Create(smartMeterId, smartMeterCreateDto.Name, metadataList);
            await smartMeterRepository.AddAsync(smartMeter);

            string topic = $"smartmeter/{smartMeterId}";
            string username = $"smartmeter-{smartMeterId}";
            string password = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
            await vaultRepository.SaveMqttBrokerCredentialsAsync(smartMeterId, topic, username, password);
            await mqttBrokerRepository.CreateMqttUserAsync(topic, username, password);
        });


        return smartMeterId.Id;
    }

    public async Task<Guid> AddMetadataAsync(Guid smartMeterId, MetadataCreateDto metadataCreateDto)
    {
        var smartMeter =
            await smartMeterRepository.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId));

        if (smartMeter == null)
        {
            logger.LogError("Smart meter with id '{SmartMeterId} not found.", smartMeterId);
            throw new SmartMeterNotFoundException(smartMeterId);
        }

        var metadataId = smartMeterRepository.NextMetadataIdentity();
        var locationDto = metadataCreateDto.Location;
        var location = locationDto != null
            ? new Location(locationDto.StreetName, locationDto.City,
                locationDto.State, locationDto.Country, locationDto.Continent)
            : null;
        var metadata = Metadata.Create(metadataId, metadataCreateDto.ValidFrom, location,
            metadataCreateDto.HouseholdSize, smartMeter.Id);

        try
        {
            smartMeter.AddMetadata(metadata);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Metadata with id '{MetadataId}' already exists", metadataId.Id);
            throw new MetadataAlreadyExistsException(metadataId.Id);
        }

        await smartMeterRepository.UpdateAsync(smartMeter);

        return smartMeterId;
    }
}