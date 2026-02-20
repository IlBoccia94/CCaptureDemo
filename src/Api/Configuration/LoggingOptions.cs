namespace Api.Configuration;

public sealed class LoggingOptions
{
    public const string SectionName = "LoggingOptions";

    public string ApplicationName { get; init; } = "EnterpriseAiDocumentProcessing.Api";
    public bool IncludeScopes { get; init; } = false;
}
