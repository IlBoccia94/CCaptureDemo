using Application.Common.Interfaces;
using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using Domain.Entities;

namespace Application.DocumentProcessing.Services;

public sealed class DocumentProcessingOrchestrator(
    IRepository<Document> documentRepository,
    IRepository<DocumentImage> imageRepository,
    IRepository<ExtractedMetadata> metadataRepository,
    IRepository<ProcessingLog> logRepository,
    IUnitOfWork unitOfWork,
    IImagePreparationService imagePreparationService,
    IVertexDetectionService vertexDetectionService,
    IDocumentAiExtractionService documentAiExtractionService,
    IOverlayRenderer overlayRenderer,
    IImageCropper imageCropper) : IDocumentProcessingOrchestrator
{
    public async Task ProcessAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new InvalidOperationException($"Document {documentId} not found.");

        try
        {
            document.MarkProcessing();
            await AddLogAsync(documentId, "processing", "Document processing started.", false, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var preparedImages = await imagePreparationService.PrepareAsync(documentId, document.StoragePath, document.FileType, cancellationToken);

            foreach (var preparedImage in preparedImages)
            {
                var imageEntity = new DocumentImage(documentId, preparedImage.ImagePath, preparedImage.PageNumber);
                await imageRepository.AddAsync(imageEntity, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                var detections = await vertexDetectionService.DetectAsync(preparedImage.ImagePath, cancellationToken);
                var accepted = detections
                    .Where(x => x.Score > 0.3m)
                    .GroupBy(x => x.Label)
                    .Select(group => group.OrderByDescending(x => x.Score).First())
                    .ToList();

                foreach (var detection in accepted)
                {
                    var croppedPath = BuildCroppedPath(preparedImage.ImagePath, documentId, preparedImage.PageNumber, detection.Label);
                    await imageCropper.CropAsync(preparedImage.ImagePath, croppedPath, detection.BoundingBox, cancellationToken);
                    imageEntity.SetDetection(detection.Label, detection.Score, croppedPath);
                    imageRepository.Update(imageEntity);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    var extractedFields = await documentAiExtractionService.ExtractAsync(croppedPath, detection.Label, cancellationToken);

                    foreach (var field in extractedFields)
                    {
                        var metadata = new ExtractedMetadata(
                            documentId,
                            imageEntity.Id,
                            field.Name,
                            field.Value,
                            field.Confidence,
                            (int?)field.BoundingBox?.X,
                            (int?)field.BoundingBox?.Y,
                            (int?)field.BoundingBox?.Width,
                            (int?)field.BoundingBox?.Height);

                        await metadataRepository.AddAsync(metadata, cancellationToken);
                    }

                    var overlayPath = Path.Combine(
                        Path.GetDirectoryName(croppedPath)!,
                        $"overlay_{Path.GetFileName(croppedPath)}");

                    await overlayRenderer.RenderAsync(croppedPath, overlayPath, extractedFields, cancellationToken);
                    imageEntity.SetOverlayPath(overlayPath);
                    imageRepository.Update(imageEntity);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            document.MarkProcessed();
            await AddLogAsync(documentId, "processing", "Document processing completed.", false, cancellationToken);
        }
        catch (Exception ex)
        {
            document.MarkFailed(ex.Message);
            await AddLogAsync(documentId, "processing", ex.ToString(), true, cancellationToken);
        }

        documentRepository.Update(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string BuildCroppedPath(string sourcePath, Guid documentId, int pageNumber, Domain.Enums.DetectedLabel label)
    {
        var directory = Path.Combine(Path.GetDirectoryName(sourcePath)!, "cropped");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, $"{documentId}_p{pageNumber}_{label}.png");
    }

    private async Task AddLogAsync(Guid documentId, string step, string message, bool isError, CancellationToken cancellationToken)
    {
        await logRepository.AddAsync(new ProcessingLog(documentId, step, message, isError), cancellationToken);
    }
}
