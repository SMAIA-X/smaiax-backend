namespace SMAIAXBackend.Application.Exceptions;

public class MetadataIdMismatchException(Guid metadataIdExpected, Guid metadataIdActual) : Exception
{
    public override string Message { get; } = $"MetadataId `{metadataIdExpected}` in the path does not match the MetadataId `{metadataIdActual}` in the body.";
}