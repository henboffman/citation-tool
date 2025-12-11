namespace CitationTool.Client.Models;

public class ImportResult
{
    public bool Success { get; set; }
    public int TotalRecords { get; set; }
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ImportError> Errors { get; set; } = new();
    public List<Citation> ImportedCitations { get; set; } = new();
}

public class ImportError
{
    public int Row { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ExportOptions
{
    public ExportFormat Format { get; set; } = ExportFormat.Json;
    public bool IncludeAllFields { get; set; } = true;
    public List<string> SelectedFields { get; set; } = new();
    public Guid? FilterByDomainId { get; set; }
    public List<CitationType>? FilterByTypes { get; set; }
}

public enum ExportFormat
{
    Json,
    Csv,
    BibTeX
}

public enum ImportFormat
{
    Json,
    Csv,
    BibTeX
}

public class AppSettings
{
    public int UrlHealthCheckIntervalMinutes { get; set; } = 60;
    public bool AutoBackupEnabled { get; set; } = true;
    public int AutoBackupIntervalDays { get; set; } = 7;
    public DateTime? LastBackupDate { get; set; }
    public bool DarkMode { get; set; } = false;
}
