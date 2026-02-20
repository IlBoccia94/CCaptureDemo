using Domain.Enums;

namespace Application.DocumentProcessing.Interfaces;

public interface IStorageService
{
    Task<(string StoredPath, DocumentType FileType, string StoredFileName)> SaveOriginalAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
    string GetDocumentWorkingDirectory(Guid documentId);
}
