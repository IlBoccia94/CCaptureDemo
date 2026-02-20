using Application.DocumentProcessing.Dtos;
using ImageMagick;

namespace Infrastructure.Processing;

internal static class ImageMagickHelpers
{
    public static Task CropAsync(string sourcePath, string outputPath, BoundingBox box, CancellationToken cancellationToken)
    {
        using var image = new MagickImage(sourcePath);
        var x = (uint)Math.Max(0, box.X);
        var y = (uint)Math.Max(0, box.Y);
        var width = (uint)Math.Max(1, Math.Min((int)(image.Width - x), box.Width));
        var height = (uint)Math.Max(1, Math.Min((int)(image.Height - y), box.Height));
        image.Crop(new MagickGeometry(x, y, width, height));
        image.Write(outputPath);
        return Task.CompletedTask;
    }
}
