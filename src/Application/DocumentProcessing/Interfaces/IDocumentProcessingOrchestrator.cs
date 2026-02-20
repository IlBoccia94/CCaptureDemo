namespace Application.DocumentProcessing.Interfaces;

public interface IDocumentProcessingOrchestrator
{
    Task ProcessAsync(Guid documentId, CancellationToken cancellationToken);
}
