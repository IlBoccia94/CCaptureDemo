using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using ImageMagick;

namespace Infrastructure.Processing;

public sealed class OverlayRenderer : IOverlayRenderer
{
    public Task<string> RenderAsync(string inputImagePath, string outputImagePath, IReadOnlyCollection<ExtractedField> fields, CancellationToken cancellationToken)
    {
        using var image = new MagickImage(inputImagePath);
        using var drawables = new Drawables().StrokeColor(MagickColors.Lime).StrokeWidth(2).FillColor(MagickColors.Transparent);

        foreach (var field in fields.Where(f => f.BoundingBox is not null))
        {
            var box = field.BoundingBox!;
            drawables.Rectangle(box.X, box.Y, box.X + box.Width, box.Y + box.Height)
                .FillColor(MagickColors.Lime)
                .FontPointSize(18)
                .Text(box.X + 2, Math.Max(20, box.Y - 4), field.Name)
                .FillColor(MagickColors.Transparent);
        }

        drawables.Draw(image);
        image.Write(outputImagePath);
        return Task.FromResult(outputImagePath);
    }
}
