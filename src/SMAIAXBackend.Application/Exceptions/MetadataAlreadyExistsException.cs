namespace SMAIAXBackend.Application.Exceptions;

public class MetadataAlreadyExistsException(Guid metadataId) : Exception
{
    public override string Message { get; } = $"Metadata with id '{metadataId}' already exists";
}