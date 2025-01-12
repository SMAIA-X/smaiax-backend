using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;
using SMAIAXBackend.Domain.Repositories.Transactions;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterCreateServiceTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IMqttBrokerRepository> _mqttBrokerRepositoryMock;
    private Mock<IVaultRepository> _vaultRepositoryMock;
    private Mock<ITransactionManager> _transactionManagerMock;
    private Mock<ILogger<SmartMeterCreateService>> _loggerMock;
    private SmartMeterCreateService _smartMeterCreateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _mqttBrokerRepositoryMock = new Mock<IMqttBrokerRepository>();
        _vaultRepositoryMock = new Mock<IVaultRepository>();
        _transactionManagerMock = new Mock<ITransactionManager>();
        _loggerMock = new Mock<ILogger<SmartMeterCreateService>>();
        _smartMeterCreateService = new SmartMeterCreateService(_smartMeterRepositoryMock.Object,
            _mqttBrokerRepositoryMock.Object, _vaultRepositoryMock.Object, _transactionManagerMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterAssignDto_WhenAssignSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var serialNumber = new ConnectorSerialNumber(Guid.NewGuid());
        var smartMeterIdExpected = new SmartMeterId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(smartMeterIdExpected, "test", serialNumber, "");
        var smartMeterAssignDto = new SmartMeterAssignDto(serialNumber.SerialNumber, "Test Smart Meter", null);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterBySerialNumberAsync(serialNumber)).ReturnsAsync(smartMeter);

        // When
        var smartMeterIdActual =
            await _smartMeterCreateService.AssignSmartMeterAsync(smartMeterAssignDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected.Id));
    }

    [Test]
    public async Task GivenSmartMeterAssignDtoWithMetadata_WhenAssignSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var serialNumber = new ConnectorSerialNumber(Guid.NewGuid());
        var smartMeterIdExpected = new SmartMeterId(Guid.NewGuid());
        var smartMeter = SmartMeter.Create(smartMeterIdExpected, "test", serialNumber, "");
        var smartMeterAssignDto = new SmartMeterAssignDto(serialNumber.SerialNumber, "Test Smart Meter",
            new MetadataCreateDto(DateTime.UtcNow,
                new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1));

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterBySerialNumberAsync(serialNumber)).ReturnsAsync(smartMeter);

        // When
        var smartMeterIdActual =
            await _smartMeterCreateService.AssignSmartMeterAsync(smartMeterAssignDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected.Id));
    }

    [Test]
    public void GivenSmartMeterAssignDto_WhenAssignSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var serialNumber = new ConnectorSerialNumber(Guid.NewGuid());
        var smartMeterCreateDto = new SmartMeterAssignDto(serialNumber.SerialNumber, "Test Smart Meter", null);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterBySerialNumberAsync(serialNumber))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterCreateService.AssignSmartMeterAsync(smartMeterCreateDto));
    }

    [Test]
    public async Task
        GivenSmartMeterIdAndMetadataCreateDto_WhenAddMetadata_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataIdExpected = new MetadataId(Guid.NewGuid());
        var metadataCreateDto = new MetadataCreateDto(DateTime.UtcNow,
            new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1);
        var smartMeter = SmartMeter.Create(smartMeterId, "Test Smart Meter", []);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(smartMeterId))
            .ReturnsAsync(smartMeter);
        _smartMeterRepositoryMock.Setup(repo => repo.NextMetadataIdentity()).Returns(metadataIdExpected);

        // When
        var smartMeterIdActual =
            await _smartMeterCreateService.AddMetadataAsync(smartMeterId.Id, metadataCreateDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterId.Id));
    }

    [Test]
    public void
        GivenSmartMeterIdAndMetadataCreateDto_WhenAddMetadata_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataCreateDto = new MetadataCreateDto(DateTime.UtcNow,
            new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(smartMeterId))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterCreateService.AddMetadataAsync(smartMeterId.Id, metadataCreateDto));
    }

    [Test]
    public void
        GivenSmartMeterIdAndExistingMetadataCreateDto_WhenAddMetadata_ThenMetadataAlreadyExistsExceptionIsThrown()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var metadataId = new MetadataId(Guid.NewGuid());
        var metadataCreateDto = new MetadataCreateDto(DateTime.UtcNow,
            new LocationDto("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1);
        var smartMeter = SmartMeter.Create(smartMeterId, "Test Smart Meter", []);
        var existingMetadata = Metadata.Create(metadataId, DateTime.UtcNow,
            new Location("Test Street", "Test City", "Test State", "Test Country", Continent.Europe), 1, smartMeterId);

        smartMeter.AddMetadata(existingMetadata);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(smartMeterId))
            .ReturnsAsync(smartMeter);
        _smartMeterRepositoryMock.Setup(repo => repo.NextMetadataIdentity()).Returns(metadataId);

        // When ... Then
        Assert.ThrowsAsync<MetadataAlreadyExistsException>(async () =>
            await _smartMeterCreateService.AddMetadataAsync(smartMeterId.Id, metadataCreateDto));
    }
}