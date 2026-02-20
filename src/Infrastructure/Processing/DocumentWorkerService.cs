using Application.DocumentProcessing.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Processing;

public sealed class DocumentWorkerService(
    IBackgroundTaskQueue queue,
    IDocumentProcessingOrchestrator orchestrator,
    ILogger<DocumentWorkerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var documentId = await queue.DequeueAsync(stoppingToken);

            var attempt = 0;
            const int maxAttempts = 3;
            while (attempt < maxAttempts)
            {
                attempt++;
                try
                {
                    await orchestrator.ProcessAsync(documentId, stoppingToken);
                    break;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning(ex, "Processing attempt {Attempt} failed for document {DocumentId}.", attempt, documentId);
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Processing failed for document {DocumentId} after retries.", documentId);
                    break;
                }
            }
        }
    }
}
