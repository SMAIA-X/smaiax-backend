namespace SMAIAXBackend.Application.Exceptions;

public class ExistingPoliciesException(Guid smartMeterId) : Exception
{
    public override string Message { get; } =
        $"Cannot update metadata because there are existing policies for smart meter with id '{smartMeterId}'";
}