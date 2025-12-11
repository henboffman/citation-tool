using System.Text.Json;
using CitationTool.Shared.Models;

namespace CitationTool.McpServer.Services;

/// <summary>
/// Service for loading and querying citation data from JSON files.
/// Supports the standard CitationExport format used by the web app.
/// </summary>
public class CitationDataService
{
    private readonly string _dataPath;
    private List<Citation> _citations = new();
    private List<Domain> _domains = new();
    private DateTime _lastLoaded = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromSeconds(30);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CitationDataService(string dataPath)
    {
        _dataPath = dataPath;
    }

    public async Task<bool> LoadDataAsync()
    {
        if (DateTime.UtcNow - _lastLoaded < _cacheExpiry && _citations.Count > 0)
            return true;

        if (!File.Exists(_dataPath))
        {
            Console.Error.WriteLine($"[CitationMCP] Data file not found: {_dataPath}");
            return false;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_dataPath);
            var export = JsonSerializer.Deserialize<CitationExport>(json, JsonOptions);

            if (export != null)
            {
                _citations = export.Citations;
                _domains = export.Domains;
                _lastLoaded = DateTime.UtcNow;
                Console.Error.WriteLine($"[CitationMCP] Loaded {_citations.Count} citations and {_domains.Count} domains");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[CitationMCP] Error loading data: {ex.Message}");
        }

        return false;
    }

    public async Task<List<Citation>> GetAllCitationsAsync()
    {
        await LoadDataAsync();
        return _citations;
    }

    public async Task<List<Domain>> GetAllDomainsAsync()
    {
        await LoadDataAsync();
        return _domains;
    }

    public async Task<Citation?> GetCitationByIdAsync(Guid id)
    {
        await LoadDataAsync();
        return _citations.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Citation?> GetCitationByIdAsync(string idString)
    {
        if (Guid.TryParse(idString, out var id))
            return await GetCitationByIdAsync(id);
        return null;
    }

    public async Task<Domain?> GetDomainByIdAsync(Guid id)
    {
        await LoadDataAsync();
        return _domains.FirstOrDefault(d => d.Id == id);
    }

    public async Task<List<Citation>> SearchCitationsAsync(
        string? query = null,
        string? domainId = null,
        string? type = null,
        int? yearFrom = null,
        int? yearTo = null,
        List<string>? tags = null,
        int limit = 50)
    {
        await LoadDataAsync();

        var results = _citations.AsEnumerable();

        // Text search across multiple fields
        if (!string.IsNullOrWhiteSpace(query))
        {
            var searchTerms = query.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            results = results.Where(c =>
                searchTerms.All(term =>
                    c.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    c.Authors.Any(a => a.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Abstract?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (c.Notes?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    c.Tags.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Doi?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                ));
        }

        // Domain filter
        if (!string.IsNullOrWhiteSpace(domainId) && Guid.TryParse(domainId, out var dId))
        {
            results = results.Where(c => c.DomainId == dId);
        }

        // Type filter
        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<CitationType>(type, true, out var citationType))
        {
            results = results.Where(c => c.Type == citationType);
        }

        // Year range filter
        if (yearFrom.HasValue)
            results = results.Where(c => c.Year >= yearFrom.Value);
        if (yearTo.HasValue)
            results = results.Where(c => c.Year <= yearTo.Value);

        // Tags filter
        if (tags?.Count > 0)
        {
            results = results.Where(c =>
                tags.All(t => c.Tags.Any(ct => ct.Equals(t, StringComparison.OrdinalIgnoreCase))));
        }

        return results.Take(limit).ToList();
    }

    public async Task<List<Citation>> GetCitationsByDomainAsync(string domainId)
    {
        await LoadDataAsync();

        if (Guid.TryParse(domainId, out var id))
        {
            return _citations.Where(c => c.DomainId == id).ToList();
        }

        // Also support domain name lookup
        var domain = _domains.FirstOrDefault(d =>
            d.Name.Equals(domainId, StringComparison.OrdinalIgnoreCase));

        if (domain != null)
        {
            return _citations.Where(c => c.DomainId == domain.Id).ToList();
        }

        return new List<Citation>();
    }

    public async Task<List<string>> GetAllTagsAsync()
    {
        await LoadDataAsync();
        return _citations
            .SelectMany(c => c.Tags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        await LoadDataAsync();

        var stats = new Dictionary<string, int>
        {
            ["total_citations"] = _citations.Count,
            ["total_domains"] = _domains.Count,
            ["total_tags"] = _citations.SelectMany(c => c.Tags).Distinct().Count()
        };

        // Count by type
        foreach (var type in Enum.GetValues<CitationType>())
        {
            var count = _citations.Count(c => c.Type == type);
            if (count > 0)
                stats[$"type_{type.ToString().ToLowerInvariant()}"] = count;
        }

        // Year range
        var years = _citations.Where(c => c.Year.HasValue).Select(c => c.Year!.Value).ToList();
        if (years.Count > 0)
        {
            stats["oldest_year"] = years.Min();
            stats["newest_year"] = years.Max();
        }

        return stats;
    }
}
