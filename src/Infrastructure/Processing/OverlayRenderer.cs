using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using ImageMagick;
using ImageMagick.Drawing;

namespace Infrastructure.Processing;

public sealed class OverlayRenderer : IOverlayRenderer
{
    public Task<string> RenderAsync(string inputImagePath, string outputImagePath, IReadOnlyCollection<ExtractedField> fields, CancellationToken cancellationToken)
    {
        using var image = new MagickImage(inputImagePath);
        var drawables = new List<IDrawable>
        {
            new DrawableStrokeColor(MagickColors.Lime),
            new DrawableStrokeWidth(2),
            new DrawableFillColor(MagickColors.Transparent)
        };

        foreach (var field in fields.Where(f => f.BoundingBox is not null))
        {
            var box = field.BoundingBox!;
            drawables.Add(new DrawableRectangle((double)box.X, (double)box.Y, (double)(box.X + box.Width), (double)(box.Y + box.Height)));
            drawables.Add(new DrawableFillColor(MagickColors.Lime));
            drawables.Add(new DrawableFontPointSize(18));
            drawables.Add(new DrawableText((double)(box.X + 2), (double)Math.Max(20, box.Y - 4), field.Name));
            drawables.Add(new DrawableFillColor(MagickColors.Transparent));
        }

        image.Draw(drawables);
        image.Write(outputImagePath);
        return Task.FromResult(outputImagePath);
    }
}
