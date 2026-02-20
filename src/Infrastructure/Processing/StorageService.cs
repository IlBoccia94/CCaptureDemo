using Application.DocumentProcessing.Interfaces;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Processing;

public sealed class StorageService(IOptions<StorageOptions> options) : IStorageService
{
    private readonly StorageOptions _options = options.Value;

    public async Task<(string StoredPath, DocumentType FileType, string StoredFileName)> SaveOriginalAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var type = extension switch
        {
            ".pdf" => DocumentType.Pdf,
            ".jpg" or ".jpeg" => DocumentType.Jpg,
            ".png" => DocumentType.Png,
            _ => throw new InvalidOperationException("Unsupported file format.")
        };

        Directory.CreateDirectory(_options.RootPath);
        var safeName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_options.RootPath, safeName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, cancellationToken);

        return (fullPath, type, safeName);
    }

    public string GetDocumentWorkingDirectory(Guid documentId)
    {
        var dir = Path.Combine(_options.RootPath, "work", documentId.ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
