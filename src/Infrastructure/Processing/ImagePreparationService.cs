using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using Domain.Enums;
using ImageMagick;

namespace Infrastructure.Processing;

public sealed class ImagePreparationService(IStorageService storageService) : IImagePreparationService
{
    private const int MaxBytes = 1_000_000;

    public Task<IReadOnlyList<PreparedImage>> PrepareAsync(Guid documentId, string originalFilePath, DocumentType documentType, CancellationToken cancellationToken)
    {
        var outputDir = Path.Combine(storageService.GetDocumentWorkingDirectory(documentId), "prepared");
        Directory.CreateDirectory(outputDir);

        var result = new List<PreparedImage>();

        if (documentType == DocumentType.Pdf)
        {
            using var pages = new MagickImageCollection();
            pages.Read(originalFilePath);

            for (var i = 0; i < pages.Count; i++)
            {
                var outPath = Path.Combine(outputDir, $"page_{i + 1}.png");
                using var image = new MagickImage(pages[i]);
                ResizeToMaxSize(image);
                image.Write(outPath, MagickFormat.Png);
                result.Add(new PreparedImage(i + 1, outPath));
            }
        }
        else
        {
            var outPath = Path.Combine(outputDir, "page_1.png");
            using var image = new MagickImage(originalFilePath);
            ResizeToMaxSize(image);
            image.Write(outPath, MagickFormat.Png);
            result.Add(new PreparedImage(1, outPath));
        }

        return Task.FromResult<IReadOnlyList<PreparedImage>>(result);
    }

    private static void ResizeToMaxSize(MagickImage image)
    {
        var quality = 90;
        while (true)
        {
            image.Quality = quality;
            using var ms = new MemoryStream();
            image.Write(ms, MagickFormat.Jpeg);
            if (ms.Length <= MaxBytes || quality <= 40)
            {
                image.Format = MagickFormat.Jpeg;
                return;
            }

            quality -= 10;
            image.Resize((int)(image.Width * 0.9), (int)(image.Height * 0.9));
        }
    }
}
