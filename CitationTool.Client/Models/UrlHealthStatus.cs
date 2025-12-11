namespace CitationTool.Client.Models;

public class UrlHealthStatus
{
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public int StatusCode { get; set; }
    public bool IsHealthy { get; set; }
    public string? ErrorMessage { get; set; }

    public HealthLevel Level => StatusCode switch
    {
        >= 200 and < 300 => HealthLevel.Healthy,
        >= 300 and < 400 => HealthLevel.Redirect,
        >= 400 and < 500 => HealthLevel.NotFound,
        >= 500 => HealthLevel.ServerError,
        0 when !string.IsNullOrEmpty(ErrorMessage) => HealthLevel.Error,
        _ => HealthLevel.Unknown
    };

    public string StatusText => Level switch
    {
        HealthLevel.Healthy => "OK",
        HealthLevel.Redirect => "Redirect",
        HealthLevel.NotFound => "Not Found",
        HealthLevel.ServerError => "Server Error",
        HealthLevel.Error => "Connection Error",
        _ => "Not Checked"
    };

    public string CssClass => Level switch
    {
        HealthLevel.Healthy => "text-success",
        HealthLevel.Redirect => "text-warning",
        HealthLevel.NotFound => "text-danger",
        HealthLevel.ServerError => "text-danger",
        HealthLevel.Error => "text-danger",
        _ => "text-secondary"
    };

    public string IconClass => Level switch
    {
        HealthLevel.Healthy => "bi-check-circle-fill",
        HealthLevel.Redirect => "bi-arrow-right-circle-fill",
        HealthLevel.NotFound => "bi-x-circle-fill",
        HealthLevel.ServerError => "bi-exclamation-triangle-fill",
        HealthLevel.Error => "bi-wifi-off",
        _ => "bi-question-circle"
    };
}

public enum HealthLevel
{
    Unknown,
    Healthy,
    Redirect,
    NotFound,
    ServerError,
    Error
}
