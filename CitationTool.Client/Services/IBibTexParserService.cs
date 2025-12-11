using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for parsing BibTeX entries into Citation objects.
/// </summary>
public interface IBibTexParserService
{
    /// <summary>
    /// Parses a BibTeX string and returns a list of citations.
    /// </summary>
    BibTexParseResult Parse(string bibtex);

    /// <summary>
    /// Parses a single BibTeX entry.
    /// </summary>
    BibTexParseResult ParseSingle(string bibtex);
}

/// <summary>
/// Result of parsing BibTeX content.
/// </summary>
public class BibTexParseResult
{
    public bool Success { get; init; }
    public List<Citation> Citations { get; init; } = new();
    public string? ErrorMessage { get; init; }
    public int ParsedCount => Citations.Count;

    public static BibTexParseResult Ok(List<Citation> citations) => new()
    {
        Success = true,
        Citations = citations
    };

    public static BibTexParseResult Ok(Citation citation) => new()
    {
        Success = true,
        Citations = new List<Citation> { citation }
    };

    public static BibTexParseResult Fail(string message) => new()
    {
        Success = false,
        ErrorMessage = message
    };
}
