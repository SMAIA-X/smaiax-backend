using Microsoft.Extensions.Logging;

using Moq;

using SMAIAXBackend.Application.DTOs;
using SMAIAXBackend.Application.Exceptions;
using SMAIAXBackend.Application.Services.Implementations;
using SMAIAXBackend.Application.Services.Interfaces;
using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Repositories;

namespace SMAIAXBackend.Application.UnitTests;

[TestFixture]
public class PolicyListServiceTests
{
    private Mock<IPolicyRepository> _policyRepositoryMock;
    private Mock<ITenantRepository> _tenantRepositoryMock;
    private Mock<ITenantContextService> _tenantContextServiceMock;
    private PolicyListService _policyListService;
    private Mock<ISmartMeterRepository> _smartMeterRepositoryMock;
    private Mock<IMeasurementListService> _measurementListServiceMock;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _policyRepositoryMock = new Mock<IPolicyRepository>();
        _smartMeterRepositoryMock = new Mock<ISmartMeterRepository>();
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _measurementListServiceMock = new Mock<IMeasurementListService>();
        _tenantContextServiceMock = new Mock<ITenantContextService>();
        _policyListService = new PolicyListService(_policyRepositoryMock.Object, _smartMeterRepositoryMock.Object,
            _tenantRepositoryMock.Object, _measurementListServiceMock.Object, _tenantContextServiceMock.Object,
            Mock.Of<ILogger<PolicyListService>>());
    }

    [Test]
    public async Task GivenExistingPolicies_WhenGetPolicies_ThenReturnPolicies()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policyId1 = new PolicyId(Guid.NewGuid());
        var policyId2 = new PolicyId(Guid.NewGuid());

        var policiesExpected = new List<Policy>
        {
            Policy.Create(policyId1, "policy1", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId),
            Policy.Create(policyId2, "policy2", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId)
        };

        _policyRepositoryMock.Setup(repo => repo.GetPoliciesAsync()).ReturnsAsync(policiesExpected);

        // When
        var policiesActual = await _policyListService.GetPoliciesAsync();

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(policiesExpected.Count));
    }

    [Test]
    public async Task GivenNoPolicies_WhenGetPolicies_ThenReturnEmptyList()
    {
        // Given
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesAsync()).ReturnsAsync(new List<Policy>());

        // When
        var policiesActual = await _policyListService.GetPoliciesAsync();

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GivenExistingPolicies_WhenGetPoliciesBySmartMeterId_ThenReturnPolicies()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policyId1 = new PolicyId(Guid.NewGuid());
        var policyId2 = new PolicyId(Guid.NewGuid());

        var policiesExpected = new List<Policy>
        {
            Policy.Create(policyId1, "policy1", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId),
            Policy.Create(policyId2, "policy2", MeasurementResolution.Hour, LocationResolution.None, 100,
                smartMeterId)
        };

        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(smartMeterId))
            .ReturnsAsync(policiesExpected);

        // When
        var policiesActual = await _policyListService.GetPoliciesBySmartMeterIdAsync(smartMeterId);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(policiesExpected.Count));
    }

    [Test]
    public async Task GivenNoPolicies_WhenGetPoliciesBySmartMeterId_ThenReturnEmptyList()
    {
        // Given
        var smartMeterId = new SmartMeterId(Guid.NewGuid());

        _policyRepositoryMock.Setup(repo => repo.GetPoliciesBySmartMeterIdAsync(smartMeterId))
            .ReturnsAsync(new List<Policy>());

        // When
        var policiesActual = await _policyListService.GetPoliciesBySmartMeterIdAsync(smartMeterId);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GivenExistingPolicies_WhenGetFilteredPolicies_ThenReturnFilteredPolicies()
    {
        // Given
        const int maxPrice = 100;
        const MeasurementResolution measurementResolution = MeasurementResolution.Hour;
        var currentTenant = Tenant.Create(new TenantId(Guid.NewGuid()), "tenant1", "tenant1");
        List<Tenant> tenants =
        [
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant2", "tenant2"),
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant3", "tenant3")
        ];

        var policyId1 = new PolicyId(Guid.NewGuid());
        var policyId2 = new PolicyId(Guid.NewGuid());

        var policiesExpected = new List<Policy>
        {
            Policy.Create(policyId1, "policy1", MeasurementResolution.Hour, LocationResolution.None, 100,
                new SmartMeterId(Guid.NewGuid())),
            Policy.Create(policyId2, "policy2", MeasurementResolution.Hour, LocationResolution.None, 100,
                new SmartMeterId(Guid.NewGuid()))
        };

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(currentTenant);
        _tenantRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tenants);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(tenants[0])).ReturnsAsync(policiesExpected);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(tenants[1])).ReturnsAsync([]);

        // When
        var policiesActual = await _policyListService.GetFilteredPoliciesAsync(maxPrice, measurementResolution, null);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(policiesExpected.Count));
    }

    [Test]
    public async Task GivenNoPolicies_WhenGetFilteredPolicies_ThenReturnEmptyList()
    {
        // Given
        const int maxPrice = 100;
        const MeasurementResolution measurementResolution = MeasurementResolution.Hour;
        var currentTenant = Tenant.Create(new TenantId(Guid.NewGuid()), "tenant1", "tenant1");
        List<Tenant> tenants =
        [
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant2", "tenant2"),
            Tenant.Create(new TenantId(Guid.NewGuid()), "tenant3", "tenant3")
        ];

        _tenantContextServiceMock.Setup(service => service.GetCurrentTenantAsync()).ReturnsAsync(currentTenant);
        _tenantRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tenants);
        _policyRepositoryMock.Setup(repo => repo.GetPoliciesByTenantAsync(It.IsAny<Tenant>())).ReturnsAsync([]);

        // When
        var policiesActual = await _policyListService.GetFilteredPoliciesAsync(maxPrice, measurementResolution, null);

        // Then
        Assert.That(policiesActual, Is.Not.Null);
        Assert.That(policiesActual, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GivenPolicy_WhenGetMeasurementsByPolicyIdAsync_ThenMeasurementsAreReturned()
    {
        // Given
        var policyId = Guid.NewGuid();
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policy = Policy.Create(new PolicyId(policyId), "policy name", MeasurementResolution.Hour,
            LocationResolution.State, 100, smartMeterId);
        _policyRepositoryMock.Setup(p => p.GetPolicyByIdAsync(It.Is<PolicyId>(id => id.Id == policyId)))
            .ReturnsAsync(policy);
        var validFrom = new DateTime(2020, 01, 01, 12, 0, 0, DateTimeKind.Utc);
        var location = new Location(null, null, "Vorarlberg", "Austria", Continent.Europe);
        var metadata = new List<Metadata>()
        {
            Metadata.Create(new MetadataId(Guid.NewGuid()), validFrom, location, null, smartMeterId)
        };
        var smartMeter = SmartMeter.Create(smartMeterId, "Smart Meter 1", metadata);
        _smartMeterRepositoryMock.Setup(rep => rep.GetSmartMeterByIdAsync(smartMeterId)).ReturnsAsync(smartMeter);
        var measurementsExpected = new List<MeasurementDto>()
        {
            new MeasurementDto(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", DateTime.UtcNow)
        };
        _measurementListServiceMock.Setup(m => m.GetMeasurementsBySmartMeterAndResolutionAsync(smartMeterId.Id,
                MeasurementResolution.Hour,
                new List<(DateTime?, DateTime?)>() { new ValueTuple<DateTime?, DateTime?>(validFrom, null) }))
            .ReturnsAsync(measurementsExpected);

        // When
        var measurementsActual = await _policyListService.GetMeasurementsByPolicyIdAsync(policyId);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GivenPolicy_WhenGetMeasurementsByPolicyIdAsyncLocationResolutionNone_ThenMeasurementsAreReturned()
    {
        // Given
        var policyId = Guid.NewGuid();
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policy = Policy.Create(new PolicyId(policyId), "policy name", MeasurementResolution.Hour,
            LocationResolution.None, 100, smartMeterId);
        _policyRepositoryMock.Setup(p => p.GetPolicyByIdAsync(It.Is<PolicyId>(id => id.Id == policyId)))
            .ReturnsAsync(policy);
        var smartMeter = SmartMeter.Create(smartMeterId, "Smart Meter 1", []);
        _smartMeterRepositoryMock.Setup(rep => rep.GetSmartMeterByIdAsync(smartMeterId)).ReturnsAsync(smartMeter);
        var measurementsExpected = new List<MeasurementDto>()
        {
            new MeasurementDto(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", DateTime.UtcNow)
        };
        _measurementListServiceMock.Setup(m => m.GetMeasurementsBySmartMeterAndResolutionAsync(smartMeterId.Id,
                MeasurementResolution.Hour, null))
            .ReturnsAsync(measurementsExpected);

        // When
        var measurementsActual = await _policyListService.GetMeasurementsByPolicyIdAsync(policyId);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GivenPolicy_WhenGetMeasurementsByPolicyIdAsyncMetadataEmpty_ThenMeasurementsAreReturned()
    {
        // Given
        var policyId = Guid.NewGuid();
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policy = Policy.Create(new PolicyId(policyId), "policy name", MeasurementResolution.Hour,
            LocationResolution.State, 100, smartMeterId);
        _policyRepositoryMock.Setup(p => p.GetPolicyByIdAsync(It.Is<PolicyId>(id => id.Id == policyId)))
            .ReturnsAsync(policy);
        var smartMeter = SmartMeter.Create(smartMeterId, "Smart Meter 1", []);
        _smartMeterRepositoryMock.Setup(rep => rep.GetSmartMeterByIdAsync(smartMeterId)).ReturnsAsync(smartMeter);
        var measurementsExpected = new List<MeasurementDto>()
        {
            new MeasurementDto(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", DateTime.UtcNow)
        };
        _measurementListServiceMock.Setup(m => m.GetMeasurementsBySmartMeterAndResolutionAsync(smartMeterId.Id,
                MeasurementResolution.Hour, null))
            .ReturnsAsync(measurementsExpected);

        // When
        var measurementsActual = await _policyListService.GetMeasurementsByPolicyIdAsync(policyId);

        // Then
        Assert.That(measurementsActual, Is.Not.Null);
        Assert.That(measurementsActual, Has.Count.EqualTo(0));
    }

    [Test]
    public void GivenNoPolicy_WhenGetMeasurementsByPolicyIdAsync_ThenPolicyNotFoundException()
    {
        // Given
        var policyId = Guid.NewGuid();
        _policyRepositoryMock.Setup(p => p.GetPolicyByIdAsync(It.Is<PolicyId>(id => id.Id == policyId)))
            .ReturnsAsync((Policy)null!);

        // When Then
        Assert.ThrowsAsync<PolicyNotFoundException>(() => _policyListService.GetMeasurementsByPolicyIdAsync(policyId));
    }

    [Test]
    public void GivenNoPolicy_WhenGetMeasurementsByPolicyIdAsync_ThenSmartMeterNotFoundException()
    {
        // Given
        var policyId = Guid.NewGuid();
        var smartMeterId = new SmartMeterId(Guid.NewGuid());
        var policy = Policy.Create(new PolicyId(policyId), "policy name", MeasurementResolution.Hour,
            LocationResolution.State, 100, smartMeterId);
        _policyRepositoryMock.Setup(p => p.GetPolicyByIdAsync(It.Is<PolicyId>(id => id.Id == policyId)))
            .ReturnsAsync(policy);
        _smartMeterRepositoryMock.Setup(rep => rep.GetSmartMeterByIdAsync(smartMeterId))
            .ReturnsAsync((SmartMeter)null!);

        // When Then
        Assert.ThrowsAsync<SmartMeterNotFoundException>(() =>
            _policyListService.GetMeasurementsByPolicyIdAsync(policyId));
    }
}