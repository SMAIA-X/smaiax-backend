using SMAIAXBackend.Domain.Model.ValueObjects.Ids;

namespace SMAIAXBackend.Domain.Model.Entities;

public sealed class Contract : IEquatable<Contract>
{
    public ContractId Id { get; } = null!;
    public DateTime CreatedAt { get; }
    public PolicyId PolicyId { get; }
    public PolicyRequestId PolicyRequestId { get; }

    private static Contract Create(
        ContractId id,
        DateTime createdAt,
        PolicyId policyId,
        PolicyRequestId policyRequestId)
    {
        return new Contract(id, createdAt, policyId, policyRequestId);
    }

    // Needed by EF Core
    private Contract()
    {
    }

    private Contract(
        ContractId id,
        DateTime createdAt,
        PolicyId policyId,
        PolicyRequestId policyRequestId)
    {
        Id = id;
        CreatedAt = createdAt;
        PolicyId = policyId;
        PolicyRequestId = policyRequestId;
    }

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

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}