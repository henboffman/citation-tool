using System.Text.Json.Serialization;

namespace CitationTool.Client.Models;

/// <summary>
/// Represents a saved search/filter configuration that can be quickly reapplied.
/// </summary>
public class SavedSearch
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User-defined name for the saved search.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of what this search finds.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The filter options to apply when this search is selected.
    /// </summary>
    public SavedFilterOptions Filters { get; set; } = new();

    /// <summary>
    /// When this saved search was created.
    /// </summary>
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this saved search was last used.
    /// </summary>
    public DateTime? LastUsed { get; set; }

    /// <summary>
    /// Number of times this search has been used.
    /// </summary>
    public int UseCount { get; set; }

    /// <summary>
    /// Order for display (lower = first).
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether to pin this search at the top of the list.
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Gets a summary of the active filters for display.
    /// </summary>
    [JsonIgnore]
    public string FilterSummary
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(Filters.SearchTerm))
                parts.Add($"\"{Filters.SearchTerm}\"");

            if (Filters.DomainId.HasValue)
                parts.Add("in domain");

            if (Filters.Types.Count > 0)
                parts.Add($"{Filters.Types.Count} type(s)");

            if (Filters.YearFrom.HasValue || Filters.YearTo.HasValue)
            {
                if (Filters.YearFrom.HasValue && Filters.YearTo.HasValue)
                    parts.Add($"{Filters.YearFrom}-{Filters.YearTo}");
                else if (Filters.YearFrom.HasValue)
                    parts.Add($"from {Filters.YearFrom}");
                else
                    parts.Add($"until {Filters.YearTo}");
            }

            if (Filters.Tags.Count > 0)
                parts.Add($"{Filters.Tags.Count} tag(s)");

            if (Filters.HealthStatus.HasValue)
                parts.Add($"health: {Filters.HealthStatus}");

            return parts.Count > 0 ? string.Join(", ", parts) : "No filters";
        }
    }
}

/// <summary>
/// Serializable version of FilterOptions for saved searches.
/// Stores domain name instead of ID for portability.
/// </summary>
public class SavedFilterOptions
{
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Domain ID - may be null if domain was deleted.
    /// </summary>
    public Guid? DomainId { get; set; }

    /// <summary>
    /// Domain name for display (stored separately in case domain is deleted).
    /// </summary>
    public string? DomainName { get; set; }

    public List<CitationType> Types { get; set; } = new();
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public HealthLevel? HealthStatus { get; set; }
    public List<string> Tags { get; set; } = new();
    public SortField SortBy { get; set; } = SortField.DateAdded;
    public bool SortDescending { get; set; } = true;

    /// <summary>
    /// Creates a SavedFilterOptions from a FilterOptions instance.
    /// </summary>
    public static SavedFilterOptions FromFilterOptions(FilterOptions options, string? domainName = null)
    {
        return new SavedFilterOptions
        {
            SearchTerm = options.SearchTerm,
            DomainId = options.DomainId,
            DomainName = domainName,
            Types = new List<CitationType>(options.Types),
            YearFrom = options.YearFrom,
            YearTo = options.YearTo,
            HealthStatus = options.HealthStatus,
            Tags = new List<string>(options.Tags),
            SortBy = options.SortBy,
            SortDescending = options.SortDescending
        };
    }

    /// <summary>
    /// Converts to a FilterOptions instance.
    /// </summary>
    public FilterOptions ToFilterOptions()
    {
        return new FilterOptions
        {
            SearchTerm = SearchTerm,
            DomainId = DomainId,
            Types = new List<CitationType>(Types),
            YearFrom = YearFrom,
            YearTo = YearTo,
            HealthStatus = HealthStatus,
            Tags = new List<string>(Tags),
            SortBy = SortBy,
            SortDescending = SortDescending
        };
    }

    /// <summary>
    /// Returns true if any filters are active.
    /// </summary>
    public bool HasActiveFilters =>
        !string.IsNullOrEmpty(SearchTerm) ||
        DomainId.HasValue ||
        Types.Count > 0 ||
        YearFrom.HasValue ||
        YearTo.HasValue ||
        HealthStatus.HasValue ||
        Tags.Count > 0;
}
