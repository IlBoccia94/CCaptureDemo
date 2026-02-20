using Domain.Common;

namespace Domain.Entities;

public sealed class ExtractedMetadata : BaseEntity
{
    public Guid DocumentId { get; private set; }
    public Guid DocumentImageId { get; private set; }
    public string FieldName { get; private set; } = string.Empty;
    public string? FieldValue { get; private set; }
    public decimal? Confidence { get; private set; }
    public int? BoundingBoxX { get; private set; }
    public int? BoundingBoxY { get; private set; }
    public int? BoundingBoxWidth { get; private set; }
    public int? BoundingBoxHeight { get; private set; }

    private ExtractedMetadata() { }

    public ExtractedMetadata(
        Guid documentId,
        Guid documentImageId,
        string fieldName,
        string? fieldValue,
        decimal? confidence,
        int? x,
        int? y,
        int? width,
        int? height)
    {
        DocumentId = documentId;
        DocumentImageId = documentImageId;
        FieldName = fieldName;
        FieldValue = fieldValue;
        Confidence = confidence;
        BoundingBoxX = x;
        BoundingBoxY = y;
        BoundingBoxWidth = width;
        BoundingBoxHeight = height;
    }
}
