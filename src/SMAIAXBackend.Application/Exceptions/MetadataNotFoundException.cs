namespace SMAIAXBackend.Application.Exceptions;

public class MetadataNotFoundException(Guid metadataId) : Exception
{
    public override string Message { get; } = $"Metadata with id '{metadataId} not found.";
}