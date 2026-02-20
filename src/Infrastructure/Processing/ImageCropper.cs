using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;

namespace Infrastructure.Processing;

public sealed class ImageCropper : IImageCropper
{
    public Task CropAsync(string sourcePath, string outputPath, BoundingBox boundingBox, CancellationToken cancellationToken)
        => ImageMagickHelpers.CropAsync(sourcePath, outputPath, boundingBox, cancellationToken);
}
