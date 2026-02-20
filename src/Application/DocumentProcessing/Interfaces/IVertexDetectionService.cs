using Application.DocumentProcessing.Dtos;

namespace Application.DocumentProcessing.Interfaces;

public interface IVertexDetectionService
{
    Task<IReadOnlyList<DetectionCandidate>> DetectAsync(string imagePath, CancellationToken cancellationToken);
}
