using System.Diagnostics.CodeAnalysis;

using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Contract : IEquatable<Contract>
{
    public ContractId Id { get; } = null!;
    public DateTime CreatedAt { get; }
    public PolicyId PolicyId { get; }

    public static Contract Create(
        ContractId id,
        DateTime createdAt,
        PolicyId policyId)
    {
        return new Contract(id, createdAt, policyId);
    }

    // Needed by EF Core
    [ExcludeFromCodeCoverage]
    private Contract()
    {
    }

    private Contract(
        ContractId id,
        DateTime createdAt,
        PolicyId policyId)
    {
        Id = id;
        CreatedAt = createdAt;
        PolicyId = policyId;
    }

    [ExcludeFromCodeCoverage]
    public bool Equals(Contract? other)
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

        return Equals((Contract)obj);
    }

    [ExcludeFromCodeCoverage]
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}