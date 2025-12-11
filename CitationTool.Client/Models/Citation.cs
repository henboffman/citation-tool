namespace CitationTool.Client.Models;

public class Citation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public CitationType Type { get; set; } = CitationType.Article;
    public string? JournalOrConference { get; set; }
    public string? Volume { get; set; }
    public string? Issue { get; set; }
    public string? Pages { get; set; }
    public int? Year { get; set; }
    public string? Month { get; set; }
    public string? Publisher { get; set; }
    public string? Doi { get; set; }
    public string? Url { get; set; }
    public string? Isbn { get; set; }
    public string? Abstract { get; set; }
    public string? Notes { get; set; }
    public List<string> Tags { get; set; } = new();
    public Guid? DomainId { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime DateModified { get; set; } = DateTime.UtcNow;
    public UrlHealthStatus? LastHealthCheck { get; set; }

    public string AuthorsDisplay => Authors.Count > 0
        ? string.Join(", ", Authors)
        : "Unknown Author";

    public string FormattedCitation => FormatIeeeStyle();

    private string FormatIeeeStyle()
    {
        var parts = new List<string>();

        if (Authors.Count > 0)
        {
            var authorStr = Authors.Count > 3
                ? $"{Authors[0]} et al."
                : string.Join(", ", Authors);
            parts.Add(authorStr);
        }

        if (!string.IsNullOrEmpty(Title))
        {
            parts.Add($"\"{Title}\"");
        }

        if (!string.IsNullOrEmpty(JournalOrConference))
        {
            var prefix = Type == CitationType.InProceedings ? "in " : "";
            parts.Add($"{prefix}{JournalOrConference}");
        }

        if (!string.IsNullOrEmpty(Volume))
        {
            var volStr = !string.IsNullOrEmpty(Issue)
                ? $"vol. {Volume}, no. {Issue}"
                : $"vol. {Volume}";
            parts.Add(volStr);
        }

        if (!string.IsNullOrEmpty(Pages))
        {
            parts.Add($"pp. {Pages}");
        }

        if (Year.HasValue)
        {
            var yearStr = !string.IsNullOrEmpty(Month)
                ? $"{Month} {Year}"
                : Year.ToString()!;
            parts.Add(yearStr);
        }

        if (!string.IsNullOrEmpty(Doi))
        {
            parts.Add($"doi: {Doi}");
        }

        return string.Join(", ", parts) + ".";
    }
}

public enum CitationType
{
    Article,
    InProceedings,
    Book,
    InBook,
    TechReport,
    Website,
    Standard,
    Patent,
    Thesis,
    Manual,
    Misc
}

public static class CitationTypeExtensions
{
    public static string GetDisplayName(this CitationType type) => type switch
    {
        CitationType.Article => "Journal Article",
        CitationType.InProceedings => "Conference Paper",
        CitationType.Book => "Book",
        CitationType.InBook => "Book Chapter",
        CitationType.TechReport => "Technical Report",
        CitationType.Website => "Website",
        CitationType.Standard => "Standard",
        CitationType.Patent => "Patent",
        CitationType.Thesis => "Thesis",
        CitationType.Manual => "Manual",
        CitationType.Misc => "Miscellaneous",
        _ => type.ToString()
    };
}
