using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DocumentProcessing.Interfaces;

public interface IStorageService
{
    Task<(string StoredPath, DocumentType FileType, string StoredFileName)> SaveOriginalAsync(IFormFile file, CancellationToken cancellationToken);
    string GetDocumentWorkingDirectory(Guid documentId);
}
