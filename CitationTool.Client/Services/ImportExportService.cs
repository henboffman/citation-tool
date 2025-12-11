using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public class ImportExportService : IImportExportService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Task<ImportResult> ImportCsvAsync(string csvContent)
    {
        var result = new ImportResult();

        try
        {
            using var reader = new StringReader(csvContent);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            var row = 1;
            while (csv.Read())
            {
                row++;
                result.TotalRecords++;

                try
                {
                    var citation = new Citation
                    {
                        Id = Guid.NewGuid(),
                        Title = csv.GetField("Title") ?? csv.GetField("title") ?? "",
                        DateAdded = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };

                    // Authors (comma-separated or semicolon-separated)
                    var authorsField = csv.GetField("Authors") ?? csv.GetField("authors") ?? csv.GetField("Author") ?? "";
                    citation.Authors = authorsField
                        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .Where(a => !string.IsNullOrEmpty(a))
                        .ToList();

                    // Type
                    var typeField = csv.GetField("Type") ?? csv.GetField("type") ?? "Misc";
                    if (Enum.TryParse<CitationType>(typeField, true, out var type))
                        citation.Type = type;

                    // Other fields
                    citation.JournalOrConference = csv.GetField("Journal") ?? csv.GetField("Conference") ?? csv.GetField("journal") ?? "";
                    citation.Volume = csv.GetField("Volume") ?? csv.GetField("volume");
                    citation.Issue = csv.GetField("Issue") ?? csv.GetField("issue");
                    citation.Pages = csv.GetField("Pages") ?? csv.GetField("pages");
                    citation.Publisher = csv.GetField("Publisher") ?? csv.GetField("publisher");
                    citation.Doi = csv.GetField("DOI") ?? csv.GetField("doi");
                    citation.Url = csv.GetField("URL") ?? csv.GetField("url");
                    citation.Isbn = csv.GetField("ISBN") ?? csv.GetField("isbn");
                    citation.Abstract = csv.GetField("Abstract") ?? csv.GetField("abstract");
                    citation.Notes = csv.GetField("Notes") ?? csv.GetField("notes");

                    // Year
                    var yearField = csv.GetField("Year") ?? csv.GetField("year");
                    if (int.TryParse(yearField, out var year))
                        citation.Year = year;

                    // Month
                    citation.Month = csv.GetField("Month") ?? csv.GetField("month");

                    // Tags
                    var tagsField = csv.GetField("Tags") ?? csv.GetField("tags") ?? "";
                    citation.Tags = tagsField
                        .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList();

                    if (string.IsNullOrWhiteSpace(citation.Title))
                    {
                        result.Errors.Add(new ImportError { Row = row, Field = "Title", Message = "Title is required" });
                        result.ErrorCount++;
                        continue;
                    }

                    if (citation.Authors.Count == 0)
                    {
                        citation.Authors.Add("Unknown");
                    }

                    result.ImportedCitations.Add(citation);
                    result.ImportedCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ImportError { Row = row, Field = "", Message = ex.Message });
                    result.ErrorCount++;
                }
            }

            result.Success = result.ImportedCount > 0;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new ImportError { Row = 0, Field = "", Message = $"Failed to parse CSV: {ex.Message}" });
        }

        return Task.FromResult(result);
    }

    public Task<ImportResult> ImportJsonAsync(string jsonContent)
    {
        var result = new ImportResult();

        try
        {
            var citations = JsonSerializer.Deserialize<List<Citation>>(jsonContent, _jsonOptions);
            if (citations == null || citations.Count == 0)
            {
                result.Errors.Add(new ImportError { Row = 0, Field = "", Message = "No citations found in JSON" });
                return Task.FromResult(result);
            }

            result.TotalRecords = citations.Count;

            foreach (var citation in citations)
            {
                if (string.IsNullOrWhiteSpace(citation.Title))
                {
                    result.ErrorCount++;
                    result.Errors.Add(new ImportError { Row = result.TotalRecords, Field = "Title", Message = "Title is required" });
                    continue;
                }

                citation.Id = Guid.NewGuid();
                citation.DateAdded = DateTime.UtcNow;
                citation.DateModified = DateTime.UtcNow;

                if (citation.Authors == null || citation.Authors.Count == 0)
                    citation.Authors = new List<string> { "Unknown" };

                if (citation.Tags == null)
                    citation.Tags = new List<string>();

                result.ImportedCitations.Add(citation);
                result.ImportedCount++;
            }

            result.Success = result.ImportedCount > 0;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new ImportError { Row = 0, Field = "", Message = $"Failed to parse JSON: {ex.Message}" });
        }

        return Task.FromResult(result);
    }

    public Task<ImportResult> ImportBibTeXAsync(string bibtexContent)
    {
        var result = new ImportResult();

        try
        {
            var entries = ParseBibTeX(bibtexContent);
            result.TotalRecords = entries.Count;

            foreach (var entry in entries)
            {
                var citation = new Citation
                {
                    Id = Guid.NewGuid(),
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Tags = new List<string>()
                };

                // Map entry type
                citation.Type = entry.Type.ToLower() switch
                {
                    "article" => CitationType.Article,
                    "inproceedings" or "conference" => CitationType.InProceedings,
                    "book" => CitationType.Book,
                    "inbook" or "incollection" => CitationType.InBook,
                    "techreport" => CitationType.TechReport,
                    "phdthesis" or "mastersthesis" => CitationType.Thesis,
                    "manual" => CitationType.Manual,
                    "misc" or "online" => CitationType.Website,
                    _ => CitationType.Misc
                };

                // Map fields
                citation.Title = entry.GetField("title");
                citation.JournalOrConference = entry.GetField("journal") ?? entry.GetField("booktitle");
                citation.Volume = entry.GetField("volume");
                citation.Issue = entry.GetField("number");
                citation.Pages = entry.GetField("pages");
                citation.Publisher = entry.GetField("publisher");
                citation.Doi = entry.GetField("doi");
                citation.Url = entry.GetField("url");
                citation.Isbn = entry.GetField("isbn");
                citation.Abstract = entry.GetField("abstract");
                citation.Notes = entry.GetField("note");
                citation.Month = entry.GetField("month");

                // Year
                var yearStr = entry.GetField("year");
                if (int.TryParse(yearStr, out var year))
                    citation.Year = year;

                // Authors
                var authorsStr = entry.GetField("author");
                if (!string.IsNullOrEmpty(authorsStr))
                {
                    citation.Authors = authorsStr
                        .Split(new[] { " and ", " AND " }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .Select(a => CleanBibTeXString(a))
                        .ToList();
                }
                else
                {
                    citation.Authors = new List<string> { "Unknown" };
                }

                citation.Title = CleanBibTeXString(citation.Title ?? "");

                if (string.IsNullOrWhiteSpace(citation.Title))
                {
                    result.ErrorCount++;
                    continue;
                }

                result.ImportedCitations.Add(citation);
                result.ImportedCount++;
            }

            result.Success = result.ImportedCount > 0;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add(new ImportError { Row = 0, Field = "", Message = $"Failed to parse BibTeX: {ex.Message}" });
        }

        return Task.FromResult(result);
    }

    public Task<string> ExportCsvAsync(IEnumerable<Citation> citations)
    {
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // Write header
        csv.WriteField("Title");
        csv.WriteField("Authors");
        csv.WriteField("Type");
        csv.WriteField("Year");
        csv.WriteField("Month");
        csv.WriteField("Journal");
        csv.WriteField("Volume");
        csv.WriteField("Issue");
        csv.WriteField("Pages");
        csv.WriteField("Publisher");
        csv.WriteField("DOI");
        csv.WriteField("URL");
        csv.WriteField("ISBN");
        csv.WriteField("Abstract");
        csv.WriteField("Notes");
        csv.WriteField("Tags");
        csv.NextRecord();

        foreach (var citation in citations)
        {
            csv.WriteField(citation.Title);
            csv.WriteField(string.Join("; ", citation.Authors));
            csv.WriteField(citation.Type.ToString());
            csv.WriteField(citation.Year?.ToString() ?? "");
            csv.WriteField(citation.Month ?? "");
            csv.WriteField(citation.JournalOrConference ?? "");
            csv.WriteField(citation.Volume ?? "");
            csv.WriteField(citation.Issue ?? "");
            csv.WriteField(citation.Pages ?? "");
            csv.WriteField(citation.Publisher ?? "");
            csv.WriteField(citation.Doi ?? "");
            csv.WriteField(citation.Url ?? "");
            csv.WriteField(citation.Isbn ?? "");
            csv.WriteField(citation.Abstract ?? "");
            csv.WriteField(citation.Notes ?? "");
            csv.WriteField(string.Join("; ", citation.Tags));
            csv.NextRecord();
        }

        return Task.FromResult(sb.ToString());
    }

    public Task<string> ExportJsonAsync(IEnumerable<Citation> citations, bool includeAll = true)
    {
        var exportData = citations.Select(c => new
        {
            c.Title,
            c.Authors,
            Type = c.Type.ToString(),
            c.Year,
            c.Month,
            Journal = c.JournalOrConference,
            c.Volume,
            c.Issue,
            c.Pages,
            c.Publisher,
            Doi = c.Doi,
            Url = c.Url,
            Isbn = c.Isbn,
            c.Abstract,
            c.Notes,
            c.Tags
        });

        var json = JsonSerializer.Serialize(exportData, _jsonOptions);
        return Task.FromResult(json);
    }

    public Task<string> ExportBibTeXAsync(IEnumerable<Citation> citations)
    {
        var sb = new StringBuilder();
        var index = 1;

        foreach (var citation in citations)
        {
            var entryType = citation.Type switch
            {
                CitationType.Article => "article",
                CitationType.InProceedings => "inproceedings",
                CitationType.Book => "book",
                CitationType.InBook => "inbook",
                CitationType.TechReport => "techreport",
                CitationType.Thesis => "phdthesis",
                CitationType.Manual => "manual",
                CitationType.Website => "misc",
                _ => "misc"
            };

            var key = GenerateBibTeXKey(citation, index++);

            sb.AppendLine($"@{entryType}{{{key},");
            sb.AppendLine($"  title = {{{EscapeBibTeX(citation.Title)}}},");
            sb.AppendLine($"  author = {{{string.Join(" and ", citation.Authors.Select(EscapeBibTeX))}}},");

            if (citation.Year.HasValue)
                sb.AppendLine($"  year = {{{citation.Year}}},");

            if (!string.IsNullOrEmpty(citation.Month))
                sb.AppendLine($"  month = {{{EscapeBibTeX(citation.Month)}}},");

            if (!string.IsNullOrEmpty(citation.JournalOrConference))
            {
                var field = citation.Type == CitationType.InProceedings ? "booktitle" : "journal";
                sb.AppendLine($"  {field} = {{{EscapeBibTeX(citation.JournalOrConference)}}},");
            }

            if (!string.IsNullOrEmpty(citation.Volume))
                sb.AppendLine($"  volume = {{{EscapeBibTeX(citation.Volume)}}},");

            if (!string.IsNullOrEmpty(citation.Issue))
                sb.AppendLine($"  number = {{{EscapeBibTeX(citation.Issue)}}},");

            if (!string.IsNullOrEmpty(citation.Pages))
                sb.AppendLine($"  pages = {{{EscapeBibTeX(citation.Pages)}}},");

            if (!string.IsNullOrEmpty(citation.Publisher))
                sb.AppendLine($"  publisher = {{{EscapeBibTeX(citation.Publisher)}}},");

            if (!string.IsNullOrEmpty(citation.Doi))
                sb.AppendLine($"  doi = {{{EscapeBibTeX(citation.Doi)}}},");

            if (!string.IsNullOrEmpty(citation.Url))
                sb.AppendLine($"  url = {{{EscapeBibTeX(citation.Url)}}},");

            if (!string.IsNullOrEmpty(citation.Isbn))
                sb.AppendLine($"  isbn = {{{EscapeBibTeX(citation.Isbn)}}},");

            if (!string.IsNullOrEmpty(citation.Abstract))
                sb.AppendLine($"  abstract = {{{EscapeBibTeX(citation.Abstract)}}},");

            sb.AppendLine("}");
            sb.AppendLine();
        }

        return Task.FromResult(sb.ToString());
    }

    private List<BibTeXEntry> ParseBibTeX(string content)
    {
        var entries = new List<BibTeXEntry>();
        var entryPattern = new Regex(@"@(\w+)\s*\{\s*([^,]*),(.+?)\}", RegexOptions.Singleline);
        var fieldPattern = new Regex(@"(\w+)\s*=\s*[\{""]([^}""]+)[\}""]", RegexOptions.Singleline);

        foreach (Match entryMatch in entryPattern.Matches(content))
        {
            var entry = new BibTeXEntry
            {
                Type = entryMatch.Groups[1].Value,
                Key = entryMatch.Groups[2].Value.Trim()
            };

            var fieldsContent = entryMatch.Groups[3].Value;
            foreach (Match fieldMatch in fieldPattern.Matches(fieldsContent))
            {
                var fieldName = fieldMatch.Groups[1].Value.ToLower();
                var fieldValue = fieldMatch.Groups[2].Value.Trim();
                entry.Fields[fieldName] = fieldValue;
            }

            entries.Add(entry);
        }

        return entries;
    }

    private static string CleanBibTeXString(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        return value
            .Replace("{", "")
            .Replace("}", "")
            .Replace("\\\"", "")
            .Replace("\\'", "'")
            .Replace("\\&", "&")
            .Trim();
    }

    private static string EscapeBibTeX(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Replace("&", "\\&").Replace("%", "\\%");
    }

    private static string GenerateBibTeXKey(Citation citation, int index)
    {
        var author = citation.Authors.FirstOrDefault()?.Split(' ').LastOrDefault() ?? "unknown";
        var year = citation.Year?.ToString() ?? "0000";
        var titleWord = citation.Title.Split(' ').FirstOrDefault()?.ToLower() ?? "ref";
        return $"{author}{year}{titleWord}_{index}";
    }

    private class BibTeXEntry
    {
        public string Type { get; set; } = "";
        public string Key { get; set; } = "";
        public Dictionary<string, string> Fields { get; set; } = new();

        public string? GetField(string name) =>
            Fields.TryGetValue(name.ToLower(), out var value) ? value : null;
    }
}
