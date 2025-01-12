using Microsoft.EntityFrameworkCore;

using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.IntegrationTests.RepositoryTests;

[TestFixture]
public class SmartMeterRepositoryTests : TestBase
{
    [Test]
    public async Task GivenSmartMeter_WhenAdd_ThenExpectedSmartMeterIsPersisted()
    {
        // Given
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(Guid.NewGuid()), "Test", []);

        // When
        await _smartMeterRepository.AddAsync(smartMeterExpected);
        var smartMeterActual = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterExpected.Id));

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
        });
    }

    [Test]
    public async Task GivenSmartMetersInRepository_WhenGetSmartMetersByUserId_ThenExpectedSmartMetersAreReturned()
    {
        // Given
        var smartMetersExpected = new List<SmartMeter>
        {
            SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")), "Smart Meter 1", []),
            SmartMeter.Create(new SmartMeterId(Guid.Parse("f4c70232-6715-4c15-966f-bf4bcef46d39")), "Smart Meter 2", [])
        };

        // When
        var smartMetersActual = await _smartMeterRepository.GetSmartMetersAsync();

        // Then
        Assert.That(smartMetersActual, Is.Not.Null);
        Assert.That(smartMetersActual, Has.Count.EqualTo(smartMetersExpected.Count + 1)); // +1 because of the seed data
    }

    [Test]
    public async Task GivenSmartMeterInRepository_WhenGetSmartMeterById_ThenExpectedSmartMeterIsReturned()
    {
        // Given
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1", []);

        // When
        var smartMeterActual = await _smartMeterRepository.GetSmartMeterByIdAsync(smartMeterExpected.Id);

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
        });
    }

    [Test]
    public async Task GivenSmartMeterInRepository_WhenGetSmartMeterBySerialNumber_ThenExpectedSmartMeterIsReturned()
    {
        // Given
        var smartMeterIdExpected = new SmartMeterId(Guid.Parse("1355836c-ba6c-4e23-b48a-72b77025bd6b"));
        var smartMeterSerialNumberExpected = new ConnectorSerialNumber(Guid.Parse("31c4fd82-5018-4bcd-bc0e-74d6b0a4e86d"));
        var smartMeterExpected = SmartMeter.Create(smartMeterIdExpected, "Smart Meter Test", smartMeterSerialNumberExpected, "");

        // When
        var smartMeterActual = await _smartMeterRepository.GetSmartMeterBySerialNumberAsync(smartMeterSerialNumberExpected);

        // Then
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Id, Is.EqualTo(smartMeterExpected.Id));
            Assert.That(smartMeterActual.ConnectorSerialNumber, Is.EqualTo(smartMeterSerialNumberExpected));
        });
    }

    [Test]
    public async Task GivenSmartMeterInRepository_WhenUpdate_ThenExpectedSmartMeterIsUpdated()
    {
        // Given
        const string name = "Smart Meter 1";
        var smartMeterExpected = SmartMeter.Create(new SmartMeterId(Guid.Parse("5e9db066-1b47-46cc-bbde-0b54c30167cd")),
            "Smart Meter 1 Updated", []);

        // When
        await _smartMeterRepository.UpdateAsync(smartMeterExpected);

        // Then
        var smartMeterActual = await _tenant1DbContext.SmartMeters
            .AsNoTracking()
            .FirstOrDefaultAsync(sm => sm.Id.Equals(smartMeterExpected.Id));
        Assert.That(smartMeterActual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(smartMeterActual.Name, Is.EqualTo(smartMeterExpected.Name));
            Assert.That(smartMeterActual.Name, Is.Not.EqualTo(name));
        });
    }
}