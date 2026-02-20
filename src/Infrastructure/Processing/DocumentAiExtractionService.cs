using System.Text;
using System.Text.Json;
using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Processing;

public sealed class DocumentAiExtractionService(
    HttpClient httpClient,
    IGoogleAccessTokenProvider tokenProvider,
    IOptions<GoogleCloudOptions> options) : IDocumentAiExtractionService
{
    private static readonly HashSet<string> AllowedFields =
    [
        "birth_date", "birth_place", "document_number", "expiry_date", "first_name", "fiscal_code", "gender",
        "issue_date", "issuing_authority", "last_name", "residence_address"
    ];

    private readonly GoogleCloudOptions _options = options.Value;

    public async Task<IReadOnlyList<ExtractedField>> ExtractAsync(string croppedImagePath, DetectedLabel label, CancellationToken cancellationToken)
    {
        var (processorId, versionId) = ResolveProcessor(label);
        var token = await tokenProvider.GetTokenAsync(cancellationToken);
        var url = $"https://{_options.Location}-documentai.googleapis.com/v1/projects/{_options.ProjectId}/locations/{_options.Location}/processors/{processorId}/processorVersions/{versionId}:process";

        var imageBytes = await File.ReadAllBytesAsync(croppedImagePath, cancellationToken);
        var body = JsonSerializer.Serialize(new
        {
            rawDocument = new
            {
                content = Convert.ToBase64String(imageBytes),
                mimeType = "image/png"
            }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var entities = json.RootElement.GetProperty("document").GetProperty("entities");

        var fields = new List<ExtractedField>();
        foreach (var entity in entities.EnumerateArray())
        {
            var type = entity.GetProperty("type").GetString() ?? string.Empty;
            if (!AllowedFields.Contains(type))
            {
                continue;
            }

            BoundingBox? box = null;
            if (entity.TryGetProperty("pageAnchor", out var anchor) &&
                anchor.TryGetProperty("pageRefs", out var refs) && refs.GetArrayLength() > 0 &&
                refs[0].TryGetProperty("boundingPoly", out var poly) &&
                poly.TryGetProperty("normalizedVertices", out var vertices) && vertices.GetArrayLength() > 1)
            {
                var first = vertices[0];
                var second = vertices[1];
                box = new BoundingBox(
                    first.GetProperty("x").GetDecimal() * 1000,
                    first.GetProperty("y").GetDecimal() * 1000,
                    Math.Abs(second.GetProperty("x").GetDecimal() - first.GetProperty("x").GetDecimal()) * 1000,
                    Math.Abs(second.GetProperty("y").GetDecimal() - first.GetProperty("y").GetDecimal()) * 1000);
            }

            fields.Add(new ExtractedField(
                type,
                entity.TryGetProperty("mentionText", out var mention) ? mention.GetString() : null,
                entity.TryGetProperty("confidence", out var confidence) ? confidence.GetDecimal() : null,
                box));
        }

        return fields;
    }

    private (string ProcessorId, string VersionId) ResolveProcessor(DetectedLabel label)
        => label switch
        {
            DetectedLabel.CicFronte or DetectedLabel.CicRetro => (_options.Processors.CicProcessorId, _options.Processors.CicVersionId),
            DetectedLabel.CieFronte or DetectedLabel.CieRetro => (_options.Processors.CieProcessorId, _options.Processors.CieVersionId),
            DetectedLabel.TsFronte or DetectedLabel.TsRetro => (_options.Processors.TsProcessorId, _options.Processors.TsVersionId),
            DetectedLabel.PatFronte => (_options.Processors.PatProcessorId, _options.Processors.PatVersionId),
            _ => throw new InvalidOperationException($"Unsupported label {label}")
        };
}
