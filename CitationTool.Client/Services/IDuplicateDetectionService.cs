using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for detecting potential duplicate citations in the library.
/// </summary>
public interface IDuplicateDetectionService
{
    /// <summary>
    /// Finds potential duplicates for a citation being added or edited.
    /// </summary>
    /// <param name="citation">The citation to check for duplicates</param>
    /// <param name="excludeId">Optional ID to exclude (for edits - don't match against self)</param>
    /// <returns>List of potential duplicates with confidence scores</returns>
    Task<List<DuplicateMatch>> FindDuplicatesAsync(Citation citation, Guid? excludeId = null);

    /// <summary>
    /// Checks if a DOI already exists in the library.
    /// </summary>
    /// <param name="doi">The DOI to check</param>
    /// <param name="excludeId">Optional ID to exclude</param>
    /// <returns>The existing citation if found, null otherwise</returns>
    Task<Citation?> FindByDoiAsync(string doi, Guid? excludeId = null);

    /// <summary>
    /// Calculates similarity between two citation titles.
    /// </summary>
    double CalculateTitleSimilarity(string title1, string title2);
}

/// <summary>
/// Represents a potential duplicate match with confidence information.
/// </summary>
public class DuplicateMatch
{
    public required Citation Citation { get; set; }
    public double Confidence { get; set; }
    public DuplicateMatchReason Reason { get; set; }
    public string ReasonDescription => Reason switch
    {
        DuplicateMatchReason.ExactDoi => "Exact DOI match",
        DuplicateMatchReason.ExactTitle => "Exact title match",
        DuplicateMatchReason.SimilarTitle => $"Similar title ({Confidence:P0} match)",
        DuplicateMatchReason.SameAuthorYearTitle => "Same author, year, and similar title",
        _ => "Potential duplicate"
    };
}

public enum DuplicateMatchReason
{
    ExactDoi,
    ExactTitle,
    SimilarTitle,
    SameAuthorYearTitle
}
