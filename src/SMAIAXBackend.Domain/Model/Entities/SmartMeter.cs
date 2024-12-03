using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class SmartMeter : IEquatable<SmartMeter>
{
    public SmartMeterId Id { get; } = null!;
    public string Name { get; private set; } = null!;
    public List<Metadata> Metadata { get; }

    public static SmartMeter Create(SmartMeterId smartMeterId, string name, List<Metadata> metadata)
    {
        return new SmartMeter(smartMeterId, name, metadata);
    }

    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private SmartMeter()
    {
    }

    private SmartMeter(SmartMeterId smartMeterId, string name, List<Metadata> metadata)
    {
        Id = smartMeterId;
        Name = name;
        Metadata = metadata;
    }

    public void AddMetadata(Metadata metadata)
    {
        if (Metadata.Contains(metadata))
        {
            throw new ArgumentException("Metadata already exists");
        }

        Metadata.Add(metadata);
    }

    public void RemoveMetadata(MetadataId metadataId)
    {
        var metadata = Metadata.Find(m => m.Id.Equals(metadataId));
        if (metadata == null)
        {
            throw new ArgumentException("Metadata not found");
        }

        Metadata.Remove(metadata);
    }

    public void UpdateMetadata(Metadata metadata)
    {
        var existingMetadata = Metadata.Find(m => m.Id.Equals(metadata.Id));
        if (existingMetadata == null)
        {
            throw new ArgumentException("Metadata not found");
        }

        Metadata.Remove(existingMetadata);
        Metadata.Add(metadata);
    }

    public void Update(string name)
    {
        Name = name;
    }

    [ExcludeFromCodeCoverage]
    public bool Equals(SmartMeter? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id.Equals(other.Id);
    }

    [ExcludeFromCodeCoverage]
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((SmartMeter)obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}