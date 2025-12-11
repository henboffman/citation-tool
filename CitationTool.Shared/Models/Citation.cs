using System.Text.Json.Serialization;

namespace CitationTool.Shared.Models;

public class Citation
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; } = new();

    [JsonPropertyName("type")]
    public CitationType Type { get; set; } = CitationType.Article;

    [JsonPropertyName("journalOrConference")]
    public string? JournalOrConference { get; set; }

    [JsonPropertyName("volume")]
    public string? Volume { get; set; }

    [JsonPropertyName("issue")]
    public string? Issue { get; set; }

    [JsonPropertyName("pages")]
    public string? Pages { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("month")]
    public string? Month { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("doi")]
    public string? Doi { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("isbn")]
    public string? Isbn { get; set; }

    [JsonPropertyName("abstract")]
    public string? Abstract { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("domainId")]
    public Guid? DomainId { get; set; }

    [JsonPropertyName("dateAdded")]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("dateModified")]
    public DateTime DateModified { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("lastHealthCheck")]
    public UrlHealthStatus? LastHealthCheck { get; set; }

    [JsonIgnore]
    public string AuthorsDisplay => Authors.Count > 0
        ? string.Join(", ", Authors)
        : "Unknown Author";

    public string FormatIeee()
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
            parts.Add($"\"{Title}\"");

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
            parts.Add($"pp. {Pages}");

        if (Year.HasValue)
        {
            var yearStr = !string.IsNullOrEmpty(Month)
                ? $"{Month} {Year}"
                : Year.ToString()!;
            parts.Add(yearStr);
        }

        if (!string.IsNullOrEmpty(Doi))
            parts.Add($"doi: {Doi}");

        return string.Join(", ", parts) + ".";
    }

    public string FormatApa()
    {
        var parts = new List<string>();

        if (Authors.Count > 0)
        {
            var authorList = Authors.Select(a =>
            {
                var nameParts = a.Split(' ');
                if (nameParts.Length >= 2)
                    return $"{nameParts.Last()}, {string.Join(" ", nameParts.Take(nameParts.Length - 1).Select(n => n[0] + "."))}";
                return a;
            });
            parts.Add(string.Join(", ", authorList));
        }

        if (Year.HasValue)
            parts.Add($"({Year})");

        if (!string.IsNullOrEmpty(Title))
            parts.Add(Title);

        if (!string.IsNullOrEmpty(JournalOrConference))
            parts.Add($"*{JournalOrConference}*");

        if (!string.IsNullOrEmpty(Volume))
            parts.Add($"*{Volume}*" + (!string.IsNullOrEmpty(Issue) ? $"({Issue})" : ""));

        if (!string.IsNullOrEmpty(Pages))
            parts.Add(Pages);

        if (!string.IsNullOrEmpty(Doi))
            parts.Add($"https://doi.org/{Doi}");

        return string.Join(". ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }

    public string FormatBibTeX()
    {
        var key = GenerateBibTeXKey();
        var entryType = Type switch
        {
            CitationType.Article => "article",
            CitationType.InProceedings => "inproceedings",
            CitationType.Book => "book",
            CitationType.InBook => "inbook",
            CitationType.TechReport => "techreport",
            CitationType.Thesis => "phdthesis",
            CitationType.Manual => "manual",
            _ => "misc"
        };

        var lines = new List<string> { $"@{entryType}{{{key}," };

        lines.Add($"  title = {{{Title}}}");
        if (Authors.Count > 0)
            lines.Add($"  author = {{{string.Join(" and ", Authors)}}}");
        if (Year.HasValue)
            lines.Add($"  year = {{{Year}}}");
        if (!string.IsNullOrEmpty(JournalOrConference))
            lines.Add($"  {(Type == CitationType.InProceedings ? "booktitle" : "journal")} = {{{JournalOrConference}}}");
        if (!string.IsNullOrEmpty(Volume))
            lines.Add($"  volume = {{{Volume}}}");
        if (!string.IsNullOrEmpty(Issue))
            lines.Add($"  number = {{{Issue}}}");
        if (!string.IsNullOrEmpty(Pages))
            lines.Add($"  pages = {{{Pages}}}");
        if (!string.IsNullOrEmpty(Publisher))
            lines.Add($"  publisher = {{{Publisher}}}");
        if (!string.IsNullOrEmpty(Doi))
            lines.Add($"  doi = {{{Doi}}}");
        if (!string.IsNullOrEmpty(Url))
            lines.Add($"  url = {{{Url}}}");
        if (!string.IsNullOrEmpty(Isbn))
            lines.Add($"  isbn = {{{Isbn}}}");

        lines.Add("}");

        return string.Join(",\n", lines.Take(lines.Count - 1)) + "\n}";
    }

    private string GenerateBibTeXKey()
    {
        var author = Authors.Count > 0 ? Authors[0].Split(' ').Last().ToLowerInvariant() : "unknown";
        var year = Year?.ToString() ?? "0000";
        var titleWord = Title.Split(' ').FirstOrDefault()?.ToLowerInvariant() ?? "untitled";
        return $"{author}{year}{titleWord}";
    }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
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
