using System.Text.RegularExpressions;
using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Parses BibTeX entries into Citation objects.
/// Supports common BibTeX entry types and fields.
/// </summary>
public partial class BibTexParserService : IBibTexParserService
{
    // Match BibTeX entries: @type{key, ... }
    [GeneratedRegex(@"@(\w+)\s*\{\s*([^,]*),\s*((?:[^{}]|\{[^{}]*\})*)\}", RegexOptions.Singleline)]
    private static partial Regex EntryPattern();

    // Match field = value pairs
    [GeneratedRegex(@"(\w+)\s*=\s*(?:\{([^{}]*(?:\{[^{}]*\}[^{}]*)*)\}|""([^""]*)""|(\d+))", RegexOptions.Singleline)]
    private static partial Regex FieldPattern();

    public BibTexParseResult Parse(string bibtex)
    {
        if (string.IsNullOrWhiteSpace(bibtex))
        {
            return BibTexParseResult.Fail("BibTeX content cannot be empty.");
        }

        var citations = new List<Citation>();
        var matches = EntryPattern().Matches(bibtex);

        if (matches.Count == 0)
        {
            return BibTexParseResult.Fail("No valid BibTeX entries found. Entries should start with @ followed by the type (e.g., @article{...).");
        }

        foreach (Match match in matches)
        {
            try
            {
                var citation = ParseEntry(match);
                if (citation != null)
                {
                    citations.Add(citation);
                }
            }
            catch (Exception ex)
            {
                // Continue parsing other entries even if one fails
                Console.WriteLine($"Error parsing BibTeX entry: {ex.Message}");
            }
        }

        if (citations.Count == 0)
        {
            return BibTexParseResult.Fail("Could not parse any valid citations from the BibTeX content.");
        }

        return BibTexParseResult.Ok(citations);
    }

    public BibTexParseResult ParseSingle(string bibtex)
    {
        var result = Parse(bibtex);

        if (!result.Success)
            return result;

        if (result.Citations.Count == 0)
            return BibTexParseResult.Fail("No citation found in BibTeX content.");

        return BibTexParseResult.Ok(result.Citations[0]);
    }

    private Citation? ParseEntry(Match match)
    {
        var entryType = match.Groups[1].Value.ToLowerInvariant();
        var citationKey = match.Groups[2].Value.Trim();
        var fieldsContent = match.Groups[3].Value;

        var fields = ParseFields(fieldsContent);

        var citation = new Citation
        {
            Type = MapEntryType(entryType),
            Title = GetField(fields, "title") ?? $"Untitled ({citationKey})",
            Authors = ParseAuthors(GetField(fields, "author")),
            Year = ParseYear(GetField(fields, "year")),
            Month = NormalizeMonth(GetField(fields, "month")),
            JournalOrConference = GetField(fields, "journal")
                ?? GetField(fields, "booktitle")
                ?? GetField(fields, "publisher"),
            Volume = GetField(fields, "volume"),
            Issue = GetField(fields, "number"),
            Pages = GetField(fields, "pages"),
            Publisher = GetField(fields, "publisher"),
            Doi = CleanDoi(GetField(fields, "doi")),
            Url = GetField(fields, "url") ?? GetField(fields, "howpublished"),
            Isbn = GetField(fields, "isbn"),
            Abstract = GetField(fields, "abstract"),
            Notes = GetField(fields, "note")
        };

        // Add keywords as tags
        var keywords = GetField(fields, "keywords");
        if (!string.IsNullOrEmpty(keywords))
        {
            citation.Tags = keywords
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();
        }

        return citation;
    }

    private Dictionary<string, string> ParseFields(string content)
    {
        var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var matches = FieldPattern().Matches(content);

        foreach (Match match in matches)
        {
            var fieldName = match.Groups[1].Value.ToLowerInvariant();
            // Value can be in braces (group 2), quotes (group 3), or bare number (group 4)
            var value = match.Groups[2].Success ? match.Groups[2].Value
                : match.Groups[3].Success ? match.Groups[3].Value
                : match.Groups[4].Value;

            // Clean up the value
            value = CleanFieldValue(value);

            if (!string.IsNullOrWhiteSpace(value))
            {
                fields[fieldName] = value;
            }
        }

        return fields;
    }

    private static string? GetField(Dictionary<string, string> fields, string name)
    {
        return fields.TryGetValue(name, out var value) ? value : null;
    }

    private static string CleanFieldValue(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Remove nested braces used for preserving case
        value = value.Replace("{", "").Replace("}", "");

        // Normalize whitespace
        value = Regex.Replace(value, @"\s+", " ").Trim();

        // Handle LaTeX special characters
        value = value
            .Replace("\\'e", "é")
            .Replace("\\'a", "á")
            .Replace("\\'i", "í")
            .Replace("\\'o", "ó")
            .Replace("\\'u", "ú")
            .Replace("\\\"a", "ä")
            .Replace("\\\"o", "ö")
            .Replace("\\\"u", "ü")
            .Replace("\\~n", "ñ")
            .Replace("\\c{c}", "ç")
            .Replace("--", "\u2013")
            .Replace("``", "\u201C")
            .Replace("''", "\u201D")
            .Replace("\\&", "&")
            .Replace("\\%", "%");

        return value;
    }

    private static List<string> ParseAuthors(string? authorField)
    {
        if (string.IsNullOrWhiteSpace(authorField))
            return new List<string>();

        // BibTeX authors are separated by " and "
        var authors = authorField
            .Split(new[] { " and ", " AND " }, StringSplitOptions.RemoveEmptyEntries)
            .Select(a => NormalizeAuthorName(a.Trim()))
            .Where(a => !string.IsNullOrEmpty(a))
            .ToList();

        return authors;
    }

    private static string NormalizeAuthorName(string author)
    {
        // Handle "Last, First" format -> "First Last"
        if (author.Contains(','))
        {
            var parts = author.Split(',', 2);
            if (parts.Length == 2)
            {
                return $"{parts[1].Trim()} {parts[0].Trim()}";
            }
        }
        return author;
    }

    private static int? ParseYear(string? yearField)
    {
        if (string.IsNullOrWhiteSpace(yearField))
            return null;

        // Extract 4-digit year from the field
        var match = Regex.Match(yearField, @"\b(19|20)\d{2}\b");
        if (match.Success && int.TryParse(match.Value, out var year))
        {
            return year;
        }

        return null;
    }

    private static string? NormalizeMonth(string? monthField)
    {
        if (string.IsNullOrWhiteSpace(monthField))
            return null;

        var month = monthField.ToLowerInvariant().Trim();

        // Handle BibTeX month macros and common formats
        return month switch
        {
            "jan" or "january" or "1" => "Jan",
            "feb" or "february" or "2" => "Feb",
            "mar" or "march" or "3" => "Mar",
            "apr" or "april" or "4" => "Apr",
            "may" or "5" => "May",
            "jun" or "june" or "6" => "Jun",
            "jul" or "july" or "7" => "Jul",
            "aug" or "august" or "8" => "Aug",
            "sep" or "september" or "9" => "Sep",
            "oct" or "october" or "10" => "Oct",
            "nov" or "november" or "11" => "Nov",
            "dec" or "december" or "12" => "Dec",
            _ => null
        };
    }

    private static string? CleanDoi(string? doi)
    {
        if (string.IsNullOrWhiteSpace(doi))
            return null;

        // Remove URL prefix if present
        doi = doi.Replace("https://doi.org/", "")
                 .Replace("http://doi.org/", "")
                 .Replace("https://dx.doi.org/", "")
                 .Replace("http://dx.doi.org/", "")
                 .Trim();

        return doi;
    }

    private static CitationType MapEntryType(string bibtexType)
    {
        return bibtexType switch
        {
            "article" => CitationType.Article,
            "inproceedings" or "conference" => CitationType.InProceedings,
            "book" => CitationType.Book,
            "inbook" or "incollection" => CitationType.InBook,
            "techreport" or "report" => CitationType.TechReport,
            "phdthesis" or "mastersthesis" or "thesis" => CitationType.Thesis,
            "manual" => CitationType.Manual,
            "misc" or "online" or "electronic" => CitationType.Website,
            "standard" => CitationType.Standard,
            _ => CitationType.Misc
        };
    }
}
