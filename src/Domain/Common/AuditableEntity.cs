namespace Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    public void SetAudit(string? createdBy, string? updatedBy)
    {
        CreatedBy = createdBy;
        UpdatedBy = updatedBy;
        MarkUpdated();
    }
}
