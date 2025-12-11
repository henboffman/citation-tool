using System.Text.Json.Serialization;

namespace CitationTool.Shared.Models;

/// <summary>
/// Standard export format for citation data.
/// This format is used for JSON exports, backups, and API responses.
/// </summary>
public class CitationExport
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("exportDate")]
    public DateTime ExportDate { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("citations")]
    public List<Citation> Citations { get; set; } = new();

    [JsonPropertyName("domains")]
    public List<Domain> Domains { get; set; } = new();

    [JsonPropertyName("metadata")]
    public ExportMetadata? Metadata { get; set; }
}

public class ExportMetadata
{
    [JsonPropertyName("citationCount")]
    public int CitationCount { get; set; }

    [JsonPropertyName("domainCount")]
    public int DomainCount { get; set; }

    [JsonPropertyName("oldestYear")]
    public int? OldestYear { get; set; }

    [JsonPropertyName("newestYear")]
    public int? NewestYear { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("citationTypes")]
    public Dictionary<string, int> CitationTypes { get; set; } = new();
}
