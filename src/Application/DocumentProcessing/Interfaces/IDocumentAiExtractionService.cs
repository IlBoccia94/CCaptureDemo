using Application.DocumentProcessing.Dtos;
using Domain.Enums;

namespace Application.DocumentProcessing.Interfaces;

public interface IDocumentAiExtractionService
{
    Task<IReadOnlyList<ExtractedField>> ExtractAsync(string croppedImagePath, DetectedLabel label, CancellationToken cancellationToken);
}
