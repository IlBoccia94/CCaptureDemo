namespace Infrastructure.Configuration;

public sealed class GoogleCloudOptions
{
    public const string SectionName = "GoogleCloud";

    public string ProjectId { get; set; } = string.Empty;
    public string Location { get; set; } = "europe-west1";
    public string ServiceAccountJsonPath { get; set; } = string.Empty;
    public string VertexEndpointId { get; set; } = string.Empty;

    public ProcessorMappingOptions Processors { get; set; } = new();
}

public sealed class ProcessorMappingOptions
{
    public string CicProcessorId { get; set; } = string.Empty;
    public string CicVersionId { get; set; } = string.Empty;
    public string CieProcessorId { get; set; } = string.Empty;
    public string CieVersionId { get; set; } = string.Empty;
    public string TsProcessorId { get; set; } = string.Empty;
    public string TsVersionId { get; set; } = string.Empty;
    public string PatProcessorId { get; set; } = string.Empty;
    public string PatVersionId { get; set; } = string.Empty;
}
