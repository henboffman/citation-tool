using System.Text.Json.Serialization;

namespace CitationTool.Shared.Models;

public class Domain
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; } = "#6c757d";

    [JsonPropertyName("dateCreated")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
