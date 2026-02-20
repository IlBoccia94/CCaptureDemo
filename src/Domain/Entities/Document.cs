using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Document : AuditableEntity
{
    public string FileName { get; private set; } = string.Empty;
    public string StoragePath { get; private set; } = string.Empty;
    public DocumentStatus Status { get; private set; } = DocumentStatus.Uploaded;

    private Document() { }

    public Document(string fileName, string storagePath)
    {
        FileName = fileName;
        StoragePath = storagePath;
        Status = DocumentStatus.Uploaded;
    }
}
