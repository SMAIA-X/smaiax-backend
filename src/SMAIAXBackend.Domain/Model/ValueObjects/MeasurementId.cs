namespace SMAIAXBackend.Domain.Model.ValueObjects;

public class MeasurementId(Guid id) : ValueObject
{
    public Guid Id { get; } = id;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }

    public override string ToString()
    {
        return Id.ToString();
    }
}