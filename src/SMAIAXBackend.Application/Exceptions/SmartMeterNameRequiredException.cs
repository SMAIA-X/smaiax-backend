namespace SMAIAXBackend.Application.Exceptions;

public class SmartMeterNameRequiredException : Exception
{
    public override string Message { get; } = "Smart meter name is required.";
}