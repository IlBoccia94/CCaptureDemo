using Application.DocumentProcessing.Dtos;
using ImageMagick;

namespace Infrastructure.Processing;

internal static class ImageMagickHelpers
{
    public static Task CropAsync(string sourcePath, string outputPath, BoundingBox box, CancellationToken cancellationToken)
    {
        using var image = new MagickImage(sourcePath);
        var x = (int)Math.Max(0, box.X);
        var y = (int)Math.Max(0, box.Y);
        var width = (int)Math.Min(image.Width - x, box.Width);
        var height = (int)Math.Min(image.Height - y, box.Height);
        image.Crop(new MagickGeometry(x, y, Math.Max(1, width), Math.Max(1, height)));
        image.Write(outputPath);
        return Task.CompletedTask;
    }
}
