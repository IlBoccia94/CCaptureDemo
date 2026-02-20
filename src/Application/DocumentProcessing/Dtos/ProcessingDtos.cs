using Domain.Enums;

namespace Application.DocumentProcessing.Dtos;

public sealed record PreparedImage(int PageNumber, string ImagePath);

public sealed record BoundingBox(decimal X, decimal Y, decimal Width, decimal Height);

public sealed record DetectionCandidate(DetectedLabel Label, decimal Score, BoundingBox BoundingBox);

public sealed record DetectionResult(DetectedLabel Label, decimal Score, BoundingBox BoundingBox, string CroppedPath);

public sealed record ExtractedField(
    string Name,
    string? Value,
    decimal? Confidence,
    BoundingBox? BoundingBox);
