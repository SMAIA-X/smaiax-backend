using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using SMAIAXBackend.Domain.Model.ValueObjects;
using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class SmartMeter : IEquatable<SmartMeter>
{
    public SmartMeterId Id { get; } = null!;

    public ConnectorSerialNumber ConnectorSerialNumber { get; private set; } = new ConnectorSerialNumber(Guid.Empty);
    public string Name { get; private set; } = "";
    public List<Metadata> Metadata { get; } = new List<Metadata>();

    public string PublicKey { get; private set; } = null!;

    public static SmartMeter Create(SmartMeterId smartMeterId, string name, List<Metadata> metadata)
    {
        return new SmartMeter(smartMeterId, name, metadata);
    }

    public static SmartMeter Create(SmartMeterId smartMeterId, string name, ConnectorSerialNumber connectorSerialNumber, string publicKey)
    {
        return new SmartMeter(smartMeterId, name, connectorSerialNumber, publicKey);
    }

    public static SmartMeter Create(SmartMeterId smartMeterId, string name, List<Metadata> metadata, ConnectorSerialNumber connectorSerialNumber, string publicKey)
    {
        return new SmartMeter(smartMeterId, name, metadata, connectorSerialNumber, publicKey);
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
    private SmartMeter(SmartMeterId smartMeterId, string name, ConnectorSerialNumber connectorSerialNumber, string publicKey)
    {
        Id = smartMeterId;
        Name = name;
        ConnectorSerialNumber = connectorSerialNumber;
        PublicKey = publicKey;
    }

    private SmartMeter(SmartMeterId smartMeterId, string name, List<Metadata> metadata, ConnectorSerialNumber connectorSerialNumber, string publicKey)
    {
        Id = smartMeterId;
        Name = name;
        Metadata = metadata;
        ConnectorSerialNumber = connectorSerialNumber;
        PublicKey = publicKey;
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