using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for detecting potential duplicate citations using DOI matching,
/// exact title matching, fuzzy title matching, and author+year+title heuristics.
/// </summary>
public class DuplicateDetectionService : IDuplicateDetectionService
{
    private readonly IStorageService _storage;

    // Minimum similarity threshold for fuzzy title matching (0.0 to 1.0)
    private const double TitleSimilarityThreshold = 0.85;

    // Higher threshold when we also have author/year match
    private const double TitleSimilarityWithContextThreshold = 0.70;

    public DuplicateDetectionService(IStorageService storage)
    {
        _storage = storage;
    }

    public async Task<List<DuplicateMatch>> FindDuplicatesAsync(Citation citation, Guid? excludeId = null)
    {
        var duplicates = new List<DuplicateMatch>();
        var allCitations = await _storage.GetAllCitationsAsync();

        // Filter out the citation being edited
        var candidates = excludeId.HasValue
            ? allCitations.Where(c => c.Id != excludeId.Value)
            : allCitations;

        foreach (var existing in candidates)
        {
            var match = CheckForDuplicate(citation, existing);
            if (match != null)
            {
                duplicates.Add(match);
            }
        }

        // Sort by confidence (highest first)
        return duplicates
            .OrderByDescending(d => d.Confidence)
            .ThenBy(d => d.Reason)
            .ToList();
    }

    public async Task<Citation?> FindByDoiAsync(string doi, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(doi))
            return null;

        var normalizedDoi = NormalizeDoi(doi);
        var allCitations = await _storage.GetAllCitationsAsync();

        return allCitations.FirstOrDefault(c =>
            c.Id != excludeId &&
            !string.IsNullOrEmpty(c.Doi) &&
            NormalizeDoi(c.Doi).Equals(normalizedDoi, StringComparison.OrdinalIgnoreCase));
    }

    public double CalculateTitleSimilarity(string title1, string title2)
    {
        if (string.IsNullOrWhiteSpace(title1) || string.IsNullOrWhiteSpace(title2))
            return 0;

        var normalized1 = NormalizeTitle(title1);
        var normalized2 = NormalizeTitle(title2);

        if (normalized1 == normalized2)
            return 1.0;

        // Use Levenshtein-based similarity
        return CalculateLevenshteinSimilarity(normalized1, normalized2);
    }

    private DuplicateMatch? CheckForDuplicate(Citation newCitation, Citation existing)
    {
        // Priority 1: Exact DOI match (100% confidence)
        if (!string.IsNullOrEmpty(newCitation.Doi) && !string.IsNullOrEmpty(existing.Doi))
        {
            if (NormalizeDoi(newCitation.Doi).Equals(NormalizeDoi(existing.Doi), StringComparison.OrdinalIgnoreCase))
            {
                return new DuplicateMatch
                {
                    Citation = existing,
                    Confidence = 1.0,
                    Reason = DuplicateMatchReason.ExactDoi
                };
            }
        }

        // Priority 2: Exact title match (after normalization)
        var newTitle = NormalizeTitle(newCitation.Title);
        var existingTitle = NormalizeTitle(existing.Title);

        if (!string.IsNullOrEmpty(newTitle) && newTitle == existingTitle)
        {
            return new DuplicateMatch
            {
                Citation = existing,
                Confidence = 0.98,
                Reason = DuplicateMatchReason.ExactTitle
            };
        }

        // Priority 3: Same author + year + similar title
        if (HasAuthorOverlap(newCitation.Authors, existing.Authors) &&
            newCitation.Year.HasValue && existing.Year.HasValue &&
            newCitation.Year == existing.Year)
        {
            var similarity = CalculateTitleSimilarity(newCitation.Title, existing.Title);
            if (similarity >= TitleSimilarityWithContextThreshold)
            {
                return new DuplicateMatch
                {
                    Citation = existing,
                    Confidence = 0.7 + (similarity * 0.25), // 70-95% confidence
                    Reason = DuplicateMatchReason.SameAuthorYearTitle
                };
            }
        }

        // Priority 4: Similar title only (high threshold)
        if (!string.IsNullOrEmpty(newTitle) && !string.IsNullOrEmpty(existingTitle))
        {
            var similarity = CalculateTitleSimilarity(newCitation.Title, existing.Title);
            if (similarity >= TitleSimilarityThreshold)
            {
                return new DuplicateMatch
                {
                    Citation = existing,
                    Confidence = similarity,
                    Reason = DuplicateMatchReason.SimilarTitle
                };
            }
        }

        return null;
    }

    private static bool HasAuthorOverlap(List<string> authors1, List<string> authors2)
    {
        if (authors1.Count == 0 || authors2.Count == 0)
            return false;

        // Check if any author's last name appears in both lists
        var lastNames1 = authors1
            .Select(GetLastName)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var lastNames2 = authors2
            .Select(GetLastName)
            .Where(n => !string.IsNullOrEmpty(n));

        return lastNames2.Any(n => lastNames1.Contains(n));
    }

    private static string GetLastName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[^1] : string.Empty;
    }

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        // Lowercase, remove punctuation, normalize whitespace
        var normalized = title.ToLowerInvariant();

        // Remove common punctuation
        normalized = new string(normalized
            .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            .ToArray());

        // Normalize whitespace
        normalized = string.Join(" ", normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        return normalized;
    }

    private static string NormalizeDoi(string doi)
    {
        if (string.IsNullOrWhiteSpace(doi))
            return string.Empty;

        var normalized = doi.Trim().ToLowerInvariant();

        // Remove common prefixes
        var prefixes = new[] { "https://doi.org/", "http://doi.org/", "doi:", "doi.org/" };
        foreach (var prefix in prefixes)
        {
            if (normalized.StartsWith(prefix))
            {
                normalized = normalized[prefix.Length..];
                break;
            }
        }

        return normalized;
    }

    /// <summary>
    /// Calculates similarity between two strings using Levenshtein distance.
    /// Returns a value between 0 (completely different) and 1 (identical).
    /// </summary>
    private static double CalculateLevenshteinSimilarity(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
            return 1.0;

        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0.0;

        var distance = LevenshteinDistance(s1, s2);
        var maxLength = Math.Max(s1.Length, s2.Length);

        return 1.0 - ((double)distance / maxLength);
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// </summary>
    private static int LevenshteinDistance(string s1, string s2)
    {
        var m = s1.Length;
        var n = s2.Length;

        // Use two rows instead of full matrix for memory efficiency
        var previousRow = new int[n + 1];
        var currentRow = new int[n + 1];

        // Initialize first row
        for (var j = 0; j <= n; j++)
            previousRow[j] = j;

        for (var i = 1; i <= m; i++)
        {
            currentRow[0] = i;

            for (var j = 1; j <= n; j++)
            {
                var cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                currentRow[j] = Math.Min(
                    Math.Min(currentRow[j - 1] + 1, previousRow[j] + 1),
                    previousRow[j - 1] + cost);
            }

            // Swap rows
            (previousRow, currentRow) = (currentRow, previousRow);
        }

        return previousRow[n];
    }
}
