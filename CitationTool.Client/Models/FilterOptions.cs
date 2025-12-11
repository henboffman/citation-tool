namespace CitationTool.Client.Models;

public class FilterOptions
{
    public string? SearchTerm { get; set; }
    public Guid? DomainId { get; set; }
    public List<CitationType> Types { get; set; } = new();
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public HealthLevel? HealthStatus { get; set; }
    public List<string> Tags { get; set; } = new();
    public SortField SortBy { get; set; } = SortField.DateAdded;
    public bool SortDescending { get; set; } = true;

    public bool HasActiveFilters =>
        !string.IsNullOrEmpty(SearchTerm) ||
        DomainId.HasValue ||
        Types.Count > 0 ||
        YearFrom.HasValue ||
        YearTo.HasValue ||
        HealthStatus.HasValue ||
        Tags.Count > 0;

    public void Clear()
    {
        SearchTerm = null;
        DomainId = null;
        Types.Clear();
        YearFrom = null;
        YearTo = null;
        HealthStatus = null;
        Tags.Clear();
    }
}

public enum SortField
{
    DateAdded,
    DateModified,
    Title,
    Year,
    Author
}

public static class SortFieldExtensions
{
    public static string GetDisplayName(this SortField field) => field switch
    {
        SortField.DateAdded => "Date Added",
        SortField.DateModified => "Date Modified",
        SortField.Title => "Title",
        SortField.Year => "Year",
        SortField.Author => "Author",
        _ => field.ToString()
    };
}
