using System.Text.Json.Serialization;

namespace CitationTool.Shared.Models;

public class UrlHealthStatus
{
    [JsonPropertyName("checkedAt")]
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("isHealthy")]
    public bool IsHealthy { get; set; }

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonIgnore]
    public HealthLevel Level => StatusCode switch
    {
        >= 200 and < 300 => HealthLevel.Healthy,
        >= 300 and < 400 => HealthLevel.Redirect,
        >= 400 and < 500 => HealthLevel.NotFound,
        >= 500 => HealthLevel.ServerError,
        0 when !string.IsNullOrEmpty(ErrorMessage) => HealthLevel.Error,
        _ => HealthLevel.Unknown
    };
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HealthLevel
{
    Unknown,
    Healthy,
    Redirect,
    NotFound,
    ServerError,
    Error
}
