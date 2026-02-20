using Application.DocumentProcessing.Dtos;
using Domain.Enums;

namespace Application.DocumentProcessing.Interfaces;

public interface IImagePreparationService
{
    Task<IReadOnlyList<PreparedImage>> PrepareAsync(Guid documentId, string originalFilePath, DocumentType documentType, CancellationToken cancellationToken);
}
