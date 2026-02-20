namespace Application.DocumentProcessing.Interfaces;

public interface IBackgroundTaskQueue
{
    ValueTask QueueDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);
    ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken);
}
