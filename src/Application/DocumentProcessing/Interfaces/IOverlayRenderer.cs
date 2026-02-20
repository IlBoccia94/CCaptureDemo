using Application.DocumentProcessing.Dtos;

namespace Application.DocumentProcessing.Interfaces;

public interface IOverlayRenderer
{
    Task<string> RenderAsync(string inputImagePath, string outputImagePath, IReadOnlyCollection<ExtractedField> fields, CancellationToken cancellationToken);
}
