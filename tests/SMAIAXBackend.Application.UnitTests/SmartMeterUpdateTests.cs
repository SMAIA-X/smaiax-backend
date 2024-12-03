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

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class SmartMeterUpdateTests
{
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ILogger<SmartMeterUpdateService>> _loggerMock;
    private SmartMeterUpdateService _smartMeterUpdateService;

    [SetUp]
    public void Setup()
    {
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _loggerMock = new Mock<ILogger<SmartMeterUpdateService>>();
        _smartMeterUpdateService = new SmartMeterUpdateService(_smartMeterRepositoryMock.Object,
            _policyRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdIsReturned()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterIdExpected), "Name", []);

        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected)))
            .ReturnsAsync(smartMeter);

        // When
        Guid smartMeterIdActual =
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto);

        // Then
        Assert.That(smartMeterIdActual, Is.EqualTo(smartMeterIdExpected));
    }

    [Test]
    public void GivenNotMatchingSmartMeterIdsAndUserId_WhenUpdateSmartMeter_ThenSmartMeterIdMismatchExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(Guid.NewGuid(), "Updated name");

        // When ... Then
        Assert.ThrowsAsync<SmartMeterIdMismatchException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto));
    }

    [Test]
    public void
        GivenNonExistentSmartMeterIdAndSmartMeterUpdateDtoAndUserId_WhenUpdateSmartMeter_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterIdExpected = Guid.NewGuid();
        var smartMeterUpdateDto = new SmartMeterUpdateDto(smartMeterIdExpected, "Updated name");

        _smartMeterRepositoryMock.Setup(repo =>
                repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterIdExpected)))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterUpdateService.UpdateSmartMeterAsync(smartMeterIdExpected, smartMeterUpdateDto));
    }

    [Test]
    public async Task GivenSmartMeterIdAndMetadataUpdateDto_WhenUpdateMetadata_ThenMetadataIdIsReturned()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var metadataId = Guid.NewGuid();
        var metadataUpdateDto = new MetadataUpdateDto(metadataId, DateTime.UtcNow,
            new LocationDto("Updated street", "Updated city", "Updated state", "Updated country", Continent.Asia), 5);
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterId), "SmartMeter", []);
        var metadata = Metadata.Create(new MetadataId(metadataId), DateTime.UtcNow,
            new Location("Some street", "Some city", "Some state", "Some country", Continent.Europe), 4, smartMeter.Id);
        smartMeter.AddMetadata(metadata);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync(smartMeter);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync([]);

        // When
        var metadataIdActual =
            await _smartMeterUpdateService.UpdateMetadataAsync(smartMeterId, metadataId, metadataUpdateDto);

        // Then
        Assert.That(metadataIdActual, Is.EqualTo(metadataId));
    }

    [Test]
    public void GivenNotMatchingMetadataIds_WhenUpdateMetadata_ThenMetadataIdMismatchExceptionIsThrown()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var metadataId = Guid.NewGuid();
        var metadataUpdateDto = new MetadataUpdateDto(Guid.NewGuid(), DateTime.UtcNow,
            new LocationDto("Updated street", "Updated city", "Updated state", "Updated country", Continent.Asia), 5);

        // When ... Then
        Assert.ThrowsAsync<MetadataIdMismatchException>(async () =>
            await _smartMeterUpdateService.UpdateMetadataAsync(smartMeterId, metadataId, metadataUpdateDto));
    }

    [Test]
    public void GivenNonExistentSmartMeterId_WhenUpdateMetadata_ThenSmartMeterNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var metadataId = Guid.NewGuid();
        var metadataUpdateDto = new MetadataUpdateDto(metadataId, DateTime.UtcNow, new LocationDto("Updated street",
            "Updated city",
            "Updated state", "Updated country", Continent.Asia), 5);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync((SmartMeter)null!);

        // When ... Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(async () =>
            await _smartMeterUpdateService.UpdateMetadataAsync(smartMeterId, metadataId, metadataUpdateDto));
    }

    [Test]
    public void GivenExistingPolicies_WhenUpdateMetadata_ThenExistingPoliciesExceptionIsThrown()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var metadataId = Guid.NewGuid();
        var metadataUpdateDto = new MetadataUpdateDto(metadataId, DateTime.UtcNow, new LocationDto("Updated street",
            "Updated city",
            "Updated state", "Updated country", Continent.Asia), 5);
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterId), "SmartMeter", []);
        var policies = new List<Policy>
        {
            Policy.Create(new PolicyId(Guid.NewGuid()), MeasurementResolution.Hour, LocationResolution.City, 10.0m,
                new SmartMeterId(smartMeterId))
        };

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync(smartMeter);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync(policies);

        // When ... Then
        Assert.ThrowsAsync<ExistingPoliciesException>(async () =>
            await _smartMeterUpdateService.UpdateMetadataAsync(smartMeterId, metadataId, metadataUpdateDto));
    }

    [Test]
    public void GivenNonExistentMetadata_WhenUpdateMetadata_ThenMetadataNotFoundExceptionIsThrown()
    {
        // Given
        var smartMeterId = Guid.NewGuid();
        var metadataId = Guid.NewGuid();
        var metadataUpdateDto = new MetadataUpdateDto(metadataId, DateTime.UtcNow, new LocationDto("Updated street", "Updated city",
            "Updated state", "Updated country", Continent.Asia), 5);
        var smartMeter = SmartMeter.Create(new SmartMeterId(smartMeterId), "SmartMeter", []);

        _smartMeterRepositoryMock.Setup(repo => repo.GetSmartMeterByIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync(smartMeter);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(new SmartMeterId(smartMeterId)))
            .ReturnsAsync([]);

        // When ... Then
        Assert.ThrowsAsync<MetadataNotFoundException>(async () =>
            await _smartMeterUpdateService.UpdateMetadataAsync(smartMeterId, metadataId, metadataUpdateDto));
    }
}