using System.Text;
using System.Text.Json;
using Application.DocumentProcessing.Dtos;
using Application.DocumentProcessing.Interfaces;
using Domain.Enums;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Processing;

public sealed class VertexDetectionService(
    HttpClient httpClient,
    IGoogleAccessTokenProvider tokenProvider,
    IOptions<GoogleCloudOptions> options) : IVertexDetectionService
{
    private readonly GoogleCloudOptions _options = options.Value;

    public async Task<IReadOnlyList<DetectionCandidate>> DetectAsync(string imagePath, CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);
        var url = $"https://{_options.Location}-aiplatform.googleapis.com/v1/projects/{_options.ProjectId}/locations/{_options.Location}/endpoints/{_options.VertexEndpointId}:predict";

        var imageBytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
        var body = JsonSerializer.Serialize(new
        {
            instances = new[] { new { content = Convert.ToBase64String(imageBytes) } }
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

        var output = new List<DetectionCandidate>();
        if (!json.RootElement.TryGetProperty("predictions", out var predictions) || predictions.GetArrayLength() == 0)
        {
            return output;
        }

        var first = predictions[0];
        var labels = first.GetProperty("displayNames").EnumerateArray().Select(x => x.GetString()).ToArray();
        var scores = first.GetProperty("confidences").EnumerateArray().Select(x => x.GetDecimal()).ToArray();
        var bboxes = first.GetProperty("bboxes").EnumerateArray().ToArray();

        for (var i = 0; i < labels.Length; i++)
        {
            if (!TryMapLabel(labels[i], out var mapped))
            {
                continue;
            }

            var bbox = bboxes[i];
            output.Add(new DetectionCandidate(
                mapped,
                scores[i],
                new BoundingBox(
                    bbox[0].GetDecimal(),
                    bbox[1].GetDecimal(),
                    bbox[2].GetDecimal(),
                    bbox[3].GetDecimal())));
        }

        return output;
    }

    private static bool TryMapLabel(string? label, out DetectedLabel mapped)
    {
        switch (label)
        {
            case "CIC_Fronte": mapped = DetectedLabel.CicFronte; return true;
            case "CIC_Retro": mapped = DetectedLabel.CicRetro; return true;
            case "CIE_Fronte": mapped = DetectedLabel.CieFronte; return true;
            case "CIE_Retro": mapped = DetectedLabel.CieRetro; return true;
            case "PAT_Fronte": mapped = DetectedLabel.PatFronte; return true;
            case "TS_Fronte": mapped = DetectedLabel.TsFronte; return true;
            case "TS_Retro": mapped = DetectedLabel.TsRetro; return true;
            default:
                mapped = default;
                return false;
        }
    }
}
