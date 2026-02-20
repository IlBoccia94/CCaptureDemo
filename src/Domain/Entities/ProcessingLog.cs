using Domain.Common;

namespace Domain.Entities;

public sealed class ProcessingLog : BaseEntity
{
    public Guid DocumentId { get; private set; }
    public string Step { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsError { get; private set; }

    private ProcessingLog() { }

    public ProcessingLog(Guid documentId, string step, string message, bool isError)
    {
        DocumentId = documentId;
        Step = step;
        Message = message;
        IsError = isError;
    }
}
