using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using ImageMagick;

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
            drawables.Add(new DrawableRectangle(box.X, box.Y, box.X + box.Width, box.Y + box.Height));
            drawables.Add(new DrawableFillColor(MagickColors.Lime));
            drawables.Add(new DrawableFontPointSize(18));
            drawables.Add(new DrawableText(box.X + 2, Math.Max(20, box.Y - 4), field.Name));
            drawables.Add(new DrawableFillColor(MagickColors.Transparent));
        }

        image.Draw(drawables);
        image.Write(outputImagePath);
        return Task.FromResult(outputImagePath);
    }
}
