using System.Threading.Channels;
using Application.DocumentProcessing.Interfaces;

namespace Infrastructure.Processing;

public sealed class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>();

    public ValueTask QueueDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(documentId, cancellationToken);

    public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAsync(cancellationToken);
}
