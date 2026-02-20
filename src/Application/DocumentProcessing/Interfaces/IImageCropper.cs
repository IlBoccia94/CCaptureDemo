using Application.DocumentProcessing.Dtos;

namespace Application.DocumentProcessing.Interfaces;

public interface IImageCropper
{
    Task CropAsync(string sourcePath, string outputPath, BoundingBox boundingBox, CancellationToken cancellationToken);
}
