using Application.Common.Interfaces;
using Application.DocumentProcessing.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/documents")]
public sealed class DocumentsController(
    IStorageService storageService,
    IRepository<Document> documentRepository,
    IRepository<DocumentImage> imageRepository,
    IRepository<ExtractedMetadata> metadataRepository,
    IRepository<ProcessingLog> logRepository,
    IUnitOfWork unitOfWork,
    IBackgroundTaskQueue backgroundTaskQueue) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Upload([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Missing file.");
        }

        var (path, type, storedFileName) = await storageService.SaveOriginalAsync(file, cancellationToken);
        var document = new Document(storedFileName, path, type);
        document.MarkQueued();

        await documentRepository.AddAsync(document, cancellationToken);
        await logRepository.AddAsync(new ProcessingLog(document.Id, "upload", "File uploaded and queued.", false), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await backgroundTaskQueue.QueueDocumentAsync(document.Id, cancellationToken);

        return Accepted(new { document.Id, document.Status });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var document = await documentRepository.GetByIdAsync(id, cancellationToken);
        if (document is null)
        {
            return NotFound();
        }

        var images = (await imageRepository.ListAsync(cancellationToken)).Where(x => x.DocumentId == id).ToList();
        var metadata = (await metadataRepository.ListAsync(cancellationToken)).Where(x => x.DocumentId == id).ToList();
        var logs = (await logRepository.ListAsync(cancellationToken)).Where(x => x.DocumentId == id).OrderBy(x => x.CreatedAtUtc).ToList();

        return Ok(new
        {
            document.Id,
            document.FileName,
            document.Status,
            document.ErrorMessage,
            document.ProcessingStartedAtUtc,
            document.ProcessingCompletedAtUtc,
            Images = images.Select(x => new { x.Id, x.PageNumber, x.Label, x.DetectionScore, x.SourcePath, x.CroppedPath, x.OverlayPath }),
            Metadata = metadata.Select(x => new { x.DocumentImageId, x.FieldName, x.FieldValue, x.Confidence }),
            Logs = logs.Select(x => new { x.Step, x.Message, x.IsError, x.CreatedAtUtc })
        });
    }
}
