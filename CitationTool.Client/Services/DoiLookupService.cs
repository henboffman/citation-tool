using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for looking up citation metadata from DOI using CrossRef API.
/// CrossRef API is free and requires no authentication.
/// See: https://api.crossref.org/swagger-ui/index.html
/// </summary>
public partial class DoiLookupService : IDoiLookupService
{
    private readonly HttpClient _httpClient;
    private const string CrossRefApiBase = "https://api.crossref.org/works/";

    // DOI regex pattern: 10.xxxx/xxxxx (where xxxx is registrant code)
    [GeneratedRegex(@"^10\.\d{4,}/[^\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex DoiPattern();

    public DoiLookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<DoiLookupResult> LookupAsync(string doi)
    {
        if (string.IsNullOrWhiteSpace(doi))
        {
            return DoiLookupResult.Fail("DOI cannot be empty.", DoiLookupErrorType.InvalidFormat);
        }

        var normalizedDoi = NormalizeDoi(doi);

        if (!IsValidDoiFormat(normalizedDoi))
        {
            return DoiLookupResult.Fail(
                "Invalid DOI format. DOI should start with '10.' followed by a registrant code and suffix (e.g., 10.1000/xyz123).",
                DoiLookupErrorType.InvalidFormat);
        }

        try
        {
            var url = $"{CrossRefApiBase}{Uri.EscapeDataString(normalizedDoi)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "CitationManager/1.0 (https://github.com/citation-tool; mailto:)");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return DoiLookupResult.Fail(
                    $"DOI '{normalizedDoi}' was not found in CrossRef. Please verify the DOI is correct.",
                    DoiLookupErrorType.NotFound);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return DoiLookupResult.Fail(
                    "Too many requests. Please wait a moment and try again.",
                    DoiLookupErrorType.RateLimited);
            }

            if (!response.IsSuccessStatusCode)
            {
                return DoiLookupResult.Fail(
                    $"CrossRef API returned an error ({response.StatusCode}). Please try again.",
                    DoiLookupErrorType.NetworkError);
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!json.TryGetProperty("message", out var message))
            {
                return DoiLookupResult.Fail(
                    "Unexpected response format from CrossRef API.",
                    DoiLookupErrorType.ParseError);
            }

            var citation = ParseCrossRefResponse(message, normalizedDoi);
            return DoiLookupResult.Ok(citation);
        }
        catch (HttpRequestException ex)
        {
            // Handle CORS or network errors gracefully
            if (ex.Message.Contains("TypeError") || ex.Message.Contains("fetch"))
            {
                return DoiLookupResult.Fail(
                    "Unable to reach CrossRef API due to browser restrictions. This may be a CORS issue.",
                    DoiLookupErrorType.NetworkError);
            }

            return DoiLookupResult.Fail(
                "Network error while contacting CrossRef. Please check your connection and try again.",
                DoiLookupErrorType.NetworkError);
        }
        catch (TaskCanceledException)
        {
            return DoiLookupResult.Fail(
                "Request timed out. CrossRef may be slow or unreachable. Please try again.",
                DoiLookupErrorType.NetworkError);
        }
        catch (JsonException ex)
        {
            return DoiLookupResult.Fail(
                $"Error parsing CrossRef response: {ex.Message}",
                DoiLookupErrorType.ParseError);
        }
        catch (Exception ex)
        {
            return DoiLookupResult.Fail(
                $"Unexpected error: {ex.Message}",
                DoiLookupErrorType.Unknown);
        }
    }

    public bool IsValidDoiFormat(string doi)
    {
        if (string.IsNullOrWhiteSpace(doi))
            return false;

        return DoiPattern().IsMatch(doi);
    }

    public string NormalizeDoi(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var doi = input.Trim();

        // Remove common prefixes
        var prefixes = new[]
        {
            "https://doi.org/",
            "http://doi.org/",
            "https://dx.doi.org/",
            "http://dx.doi.org/",
            "doi.org/",
            "doi:",
            "DOI:",
            "DOI "
        };

        foreach (var prefix in prefixes)
        {
            if (doi.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                doi = doi[prefix.Length..];
                break;
            }
        }

        return doi.Trim();
    }

    private Citation ParseCrossRefResponse(JsonElement message, string doi)
    {
        var citation = new Citation
        {
            Doi = doi,
            Title = GetTitle(message),
            Authors = GetAuthors(message),
            Type = GetCitationType(message),
            Year = GetYear(message),
            Month = GetMonth(message),
            JournalOrConference = GetContainerTitle(message),
            Volume = GetStringProperty(message, "volume"),
            Issue = GetStringProperty(message, "issue"),
            Pages = GetStringProperty(message, "page"),
            Publisher = GetStringProperty(message, "publisher"),
            Url = GetUrl(message, doi),
            Abstract = GetAbstract(message),
            Isbn = GetIsbn(message)
        };

        return citation;
    }

    private static string GetTitle(JsonElement message)
    {
        if (message.TryGetProperty("title", out var titleArray) &&
            titleArray.ValueKind == JsonValueKind.Array &&
            titleArray.GetArrayLength() > 0)
        {
            return titleArray[0].GetString() ?? string.Empty;
        }
        return string.Empty;
    }

    private static List<string> GetAuthors(JsonElement message)
    {
        var authors = new List<string>();

        if (message.TryGetProperty("author", out var authorArray) &&
            authorArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var author in authorArray.EnumerateArray())
            {
                var given = author.TryGetProperty("given", out var g) ? g.GetString() ?? "" : "";
                var family = author.TryGetProperty("family", out var f) ? f.GetString() ?? "" : "";

                if (!string.IsNullOrEmpty(family))
                {
                    var fullName = string.IsNullOrEmpty(given)
                        ? family
                        : $"{given} {family}";
                    authors.Add(fullName);
                }
            }
        }

        return authors;
    }

    private static CitationType GetCitationType(JsonElement message)
    {
        if (!message.TryGetProperty("type", out var typeElement))
            return CitationType.Misc;

        var type = typeElement.GetString()?.ToLowerInvariant();

        return type switch
        {
            "journal-article" => CitationType.Article,
            "proceedings-article" or "conference-paper" => CitationType.InProceedings,
            "book" => CitationType.Book,
            "book-chapter" => CitationType.InBook,
            "report" or "report-component" => CitationType.TechReport,
            "dissertation" => CitationType.Thesis,
            "standard" => CitationType.Standard,
            "posted-content" or "preprint" => CitationType.Article, // arXiv, etc.
            _ => CitationType.Misc
        };
    }

    private static int? GetYear(JsonElement message)
    {
        // Try published-print first, then published-online, then created
        var dateFields = new[] { "published-print", "published-online", "published", "created" };

        foreach (var field in dateFields)
        {
            if (message.TryGetProperty(field, out var dateObj) &&
                dateObj.TryGetProperty("date-parts", out var dateParts) &&
                dateParts.ValueKind == JsonValueKind.Array &&
                dateParts.GetArrayLength() > 0)
            {
                var firstDate = dateParts[0];
                if (firstDate.ValueKind == JsonValueKind.Array && firstDate.GetArrayLength() > 0)
                {
                    if (firstDate[0].TryGetInt32(out var year))
                        return year;
                }
            }
        }

        return null;
    }

    private static string? GetMonth(JsonElement message)
    {
        var dateFields = new[] { "published-print", "published-online", "published", "created" };

        foreach (var field in dateFields)
        {
            if (message.TryGetProperty(field, out var dateObj) &&
                dateObj.TryGetProperty("date-parts", out var dateParts) &&
                dateParts.ValueKind == JsonValueKind.Array &&
                dateParts.GetArrayLength() > 0)
            {
                var firstDate = dateParts[0];
                if (firstDate.ValueKind == JsonValueKind.Array && firstDate.GetArrayLength() > 1)
                {
                    if (firstDate[1].TryGetInt32(out var month) && month >= 1 && month <= 12)
                    {
                        var months = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                        return months[month];
                    }
                }
            }
        }

        return null;
    }

    private static string? GetContainerTitle(JsonElement message)
    {
        if (message.TryGetProperty("container-title", out var containerArray) &&
            containerArray.ValueKind == JsonValueKind.Array &&
            containerArray.GetArrayLength() > 0)
        {
            return containerArray[0].GetString();
        }
        return null;
    }

    private static string? GetStringProperty(JsonElement message, string propertyName)
    {
        if (message.TryGetProperty(propertyName, out var prop) &&
            prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        return null;
    }

    private static string? GetUrl(JsonElement message, string doi)
    {
        // Try to get URL from the response, fallback to DOI URL
        if (message.TryGetProperty("URL", out var urlProp) &&
            urlProp.ValueKind == JsonValueKind.String)
        {
            return urlProp.GetString();
        }

        // Fallback to standard DOI URL
        return $"https://doi.org/{doi}";
    }

    private static string? GetAbstract(JsonElement message)
    {
        if (message.TryGetProperty("abstract", out var abstractProp) &&
            abstractProp.ValueKind == JsonValueKind.String)
        {
            var text = abstractProp.GetString();

            // CrossRef abstracts often contain JATS XML tags - strip them
            if (!string.IsNullOrEmpty(text))
            {
                text = StripXmlTags(text);
                return text.Trim();
            }
        }
        return null;
    }

    private static string? GetIsbn(JsonElement message)
    {
        if (message.TryGetProperty("ISBN", out var isbnArray) &&
            isbnArray.ValueKind == JsonValueKind.Array &&
            isbnArray.GetArrayLength() > 0)
        {
            return isbnArray[0].GetString();
        }
        return null;
    }

    private static string StripXmlTags(string input)
    {
        // Remove JATS XML tags like <jats:p>, <jats:italic>, etc.
        return Regex.Replace(input, @"<[^>]+>", string.Empty);
    }
}
