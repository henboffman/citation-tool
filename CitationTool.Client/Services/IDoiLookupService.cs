using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for looking up citation metadata from DOI using CrossRef API.
/// </summary>
public interface IDoiLookupService
{
    /// <summary>
    /// Looks up citation metadata from a DOI.
    /// </summary>
    /// <param name="doi">The DOI to look up (with or without https://doi.org/ prefix)</param>
    /// <returns>Result containing the citation data or error information</returns>
    Task<DoiLookupResult> LookupAsync(string doi);

    /// <summary>
    /// Validates if a string is a valid DOI format.
    /// </summary>
    bool IsValidDoiFormat(string doi);

    /// <summary>
    /// Extracts the DOI from various input formats (URL, doi:prefix, raw DOI).
    /// </summary>
    string NormalizeDoi(string input);
}

/// <summary>
/// Result of a DOI lookup operation.
/// </summary>
public class DoiLookupResult
{
    public bool Success { get; set; }
    public Citation? Citation { get; set; }
    public string? ErrorMessage { get; set; }
    public DoiLookupErrorType ErrorType { get; set; }

    public static DoiLookupResult Ok(Citation citation) => new()
    {
        Success = true,
        Citation = citation
    };

    public static DoiLookupResult Fail(string message, DoiLookupErrorType errorType = DoiLookupErrorType.Unknown) => new()
    {
        Success = false,
        ErrorMessage = message,
        ErrorType = errorType
    };
}

public enum DoiLookupErrorType
{
    Unknown,
    InvalidFormat,
    NotFound,
    NetworkError,
    ParseError,
    RateLimited
}
