using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public sealed class DocumentImage : BaseEntity
{
    public Guid DocumentId { get; private set; }
    public string SourcePath { get; private set; } = string.Empty;
    public int PageNumber { get; private set; }
    public DetectedLabel? Label { get; private set; }
    public decimal? DetectionScore { get; private set; }
    public string? CroppedPath { get; private set; }
    public string? OverlayPath { get; private set; }

    private DocumentImage() { }

    public DocumentImage(Guid documentId, string sourcePath, int pageNumber)
    {
        DocumentId = documentId;
        SourcePath = sourcePath;
        PageNumber = pageNumber;
    }

    public void SetDetection(DetectedLabel label, decimal score, string croppedPath)
    {
        Label = label;
        DetectionScore = score;
        CroppedPath = croppedPath;
        MarkUpdated();
    }

    public void SetOverlayPath(string overlayPath)
    {
        OverlayPath = overlayPath;
        MarkUpdated();
    }
}
