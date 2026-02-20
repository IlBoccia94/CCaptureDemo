using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Document : AuditableEntity
{
    private readonly List<DocumentImage> _images = [];
    private readonly List<ExtractedMetadata> _metadata = [];
    private readonly List<ProcessingLog> _logs = [];

    public string FileName { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public DocumentType FileType { get; private set; }
    public DocumentStatus Status { get; private set; } = DocumentStatus.Uploaded;
    public string? ErrorMessage { get; private set; }
    public DateTime? ProcessingStartedAtUtc { get; private set; }
    public DateTime? ProcessingCompletedAtUtc { get; private set; }

    public IReadOnlyCollection<DocumentImage> Images => _images;
    public IReadOnlyCollection<ExtractedMetadata> Metadata => _metadata;
    public IReadOnlyCollection<ProcessingLog> Logs => _logs;

    private Document() { }

    public Document(string fileName, string storagePath, DocumentType fileType)
    {
        FileName = fileName;
        StoragePath = storagePath;
        FileType = fileType;
    }

    public void MarkQueued() => Status = DocumentStatus.Queued;

    public void MarkProcessing()
    {
        Status = DocumentStatus.Processing;
        ProcessingStartedAtUtc = DateTime.UtcNow;
        MarkUpdated();
    }

    public void MarkProcessed()
    {
        Status = DocumentStatus.Processed;
        ProcessingCompletedAtUtc = DateTime.UtcNow;
        ErrorMessage = null;
        MarkUpdated();
    }

    public void MarkFailed(string error)
    {
        Status = DocumentStatus.Failed;
        ErrorMessage = error;
        ProcessingCompletedAtUtc = DateTime.UtcNow;
        MarkUpdated();
    }

    public void AddImage(DocumentImage image) => _images.Add(image);
    public void AddMetadata(ExtractedMetadata metadata) => _metadata.Add(metadata);
    public void AddLog(ProcessingLog log) => _logs.Add(log);
}
