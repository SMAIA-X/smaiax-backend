using SMAIAXBackend.Domain.Model.Entities;
using SMAIAXBackend.Domain.Model.Enums;
using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;
using SMAIAXBackend.Domain.Specifications;

namespace SMAIAXBackend.Domain.UnitTests.Specifications;

[TestFixture]
public class MetadataLocationResolutionSpecificationTests
{
    [Test]
    public void Given_LocationResolutionIsEqual_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new MetadataLocationResolutionSpecification(LocationResolution.Country);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location(null, null, null, "USA", Continent.NorthAmerica), null, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(metadata);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_LocationResolutionIsLower_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new MetadataLocationResolutionSpecification(LocationResolution.Country);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location(null, "Washington, D.C.", "District of Columbia", "USA", Continent.NorthAmerica), 3,
            new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(metadata);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_LocationResolutionIsHigher_When_IsSatisfiedByIsCalled_Then_ReturnsFalse()
    {
        // Given
        var specification = new MetadataLocationResolutionSpecification(LocationResolution.Country);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location(null, null, null, null, Continent.Oceania),
            100, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(metadata);

        // Then
        Assert.That(result, Is.False);
    }

    [Test]
    public void Given_LocationResolutionIsNone_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new MetadataLocationResolutionSpecification(LocationResolution.None);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow,
            new Location(null, null, null, "Papua New Guinea", Continent.Oceania),
            100, new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(metadata);

        // Then
        Assert.That(result, Is.True);
    }

    [Test]
    public void Given_LocationIsNull_When_IsSatisfiedByIsCalled_Then_ReturnsFalse()
    {
        // Given
        var specification = new MetadataLocationResolutionSpecification(LocationResolution.Country);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow, null, 100,
            new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(metadata);

        // Then
        Assert.That(result, Is.False);
    }

    [Test]
    public void Given_LocationResolutionIsNoneAndLocationIsNull_When_IsSatisfiedByIsCalled_Then_ReturnsTrue()
    {
        // Given
        var specification = new MetadataLocationResolutionSpecification(LocationResolution.None);
        var metadata = Metadata.Create(new MetadataId(Guid.NewGuid()), DateTime.UtcNow, null, 100,
            new SmartMeterId(Guid.NewGuid()));

        // When
        var result = specification.IsSatisfiedBy(metadata);

        // Then
        Assert.That(result, Is.True);
    }
}