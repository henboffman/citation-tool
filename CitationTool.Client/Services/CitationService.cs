using FluentValidation;
using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public class CitationService : ICitationService
{
    private readonly IStorageService _storage;
    private readonly IValidator<Citation> _validator;
    private readonly IAppStateService _appState;

    public CitationService(IStorageService storage, IValidator<Citation> validator, IAppStateService appState)
    {
        _storage = storage;
        _validator = validator;
        _appState = appState;
    }

    public async Task<List<Citation>> GetAllAsync()
    {
        return await _storage.GetAllCitationsAsync();
    }

    public async Task<Citation?> GetByIdAsync(Guid id)
    {
        return await _storage.GetCitationAsync(id);
    }

    public async Task<ServiceResult<Citation>> CreateAsync(Citation citation)
    {
        var validationResult = await _validator.ValidateAsync(citation);
        if (!validationResult.IsValid)
        {
            return ServiceResult<Citation>.ValidationFail(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        citation.Id = Guid.NewGuid();
        citation.DateAdded = DateTime.UtcNow;
        citation.DateModified = DateTime.UtcNow;

        var success = await _storage.SaveCitationAsync(citation);
        if (!success)
        {
            return ServiceResult<Citation>.Fail(
                "Unable to save citation. Please try again. If the problem persists, try refreshing the page.");
        }

        _appState.NotifyCitationsChanged();
        return ServiceResult<Citation>.Ok(citation);
    }

    public async Task<ServiceResult<Citation>> UpdateAsync(Citation citation)
    {
        var validationResult = await _validator.ValidateAsync(citation);
        if (!validationResult.IsValid)
        {
            return ServiceResult<Citation>.ValidationFail(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var existing = await _storage.GetCitationAsync(citation.Id);
        if (existing == null)
        {
            return ServiceResult<Citation>.Fail(
                "Citation not found. It may have been deleted. Please refresh and try again.");
        }

        citation.DateModified = DateTime.UtcNow;
        var success = await _storage.SaveCitationAsync(citation);
        if (!success)
        {
            return ServiceResult<Citation>.Fail(
                "Unable to update citation. Please try again. If the problem persists, try refreshing the page.");
        }

        return ServiceResult<Citation>.Ok(citation);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var existing = await _storage.GetCitationAsync(id);
        if (existing == null)
        {
            return ServiceResult.Fail(
                "Citation not found. It may have already been deleted.");
        }

        var success = await _storage.DeleteCitationAsync(id);
        if (!success)
        {
            return ServiceResult.Fail(
                "Unable to delete citation. Please try again. If the problem persists, try refreshing the page.");
        }

        _appState.NotifyCitationsChanged();
        return ServiceResult.Ok();
    }

    public async Task<List<Citation>> SearchAsync(FilterOptions options)
    {
        var citations = await _storage.GetAllCitationsAsync();

        var query = citations.AsEnumerable();

        // Text search
        if (!string.IsNullOrWhiteSpace(options.SearchTerm))
        {
            var term = options.SearchTerm.ToLowerInvariant();
            query = query.Where(c =>
                c.Title.ToLowerInvariant().Contains(term) ||
                c.Authors.Any(a => a.ToLowerInvariant().Contains(term)) ||
                (c.Abstract?.ToLowerInvariant().Contains(term) ?? false) ||
                (c.Notes?.ToLowerInvariant().Contains(term) ?? false) ||
                c.Tags.Any(t => t.ToLowerInvariant().Contains(term)) ||
                (c.JournalOrConference?.ToLowerInvariant().Contains(term) ?? false) ||
                (c.Doi?.ToLowerInvariant().Contains(term) ?? false));
        }

        // Domain filter
        if (options.DomainId.HasValue)
        {
            query = query.Where(c => c.DomainId == options.DomainId);
        }

        // Type filter
        if (options.Types.Count > 0)
        {
            query = query.Where(c => options.Types.Contains(c.Type));
        }

        // Year range
        if (options.YearFrom.HasValue)
        {
            query = query.Where(c => c.Year >= options.YearFrom);
        }
        if (options.YearTo.HasValue)
        {
            query = query.Where(c => c.Year <= options.YearTo);
        }

        // Health status
        if (options.HealthStatus.HasValue)
        {
            query = query.Where(c =>
                c.LastHealthCheck != null &&
                c.LastHealthCheck.Level == options.HealthStatus.Value);
        }

        // Tags filter
        if (options.Tags.Count > 0)
        {
            query = query.Where(c =>
                options.Tags.All(t => c.Tags.Contains(t, StringComparer.OrdinalIgnoreCase)));
        }

        // Sorting
        query = options.SortBy switch
        {
            SortField.Title => options.SortDescending
                ? query.OrderByDescending(c => c.Title)
                : query.OrderBy(c => c.Title),
            SortField.Year => options.SortDescending
                ? query.OrderByDescending(c => c.Year ?? 0)
                : query.OrderBy(c => c.Year ?? 0),
            SortField.Author => options.SortDescending
                ? query.OrderByDescending(c => c.Authors.FirstOrDefault() ?? "")
                : query.OrderBy(c => c.Authors.FirstOrDefault() ?? ""),
            SortField.DateModified => options.SortDescending
                ? query.OrderByDescending(c => c.DateModified)
                : query.OrderBy(c => c.DateModified),
            _ => options.SortDescending
                ? query.OrderByDescending(c => c.DateAdded)
                : query.OrderBy(c => c.DateAdded)
        };

        return query.ToList();
    }

    public async Task<int> BulkImportAsync(IEnumerable<Citation> citations)
    {
        var citationList = citations.ToList();
        foreach (var citation in citationList)
        {
            if (citation.Id == Guid.Empty)
                citation.Id = Guid.NewGuid();
            if (citation.DateAdded == default)
                citation.DateAdded = DateTime.UtcNow;
            citation.DateModified = DateTime.UtcNow;
        }

        var count = await _storage.BulkAddCitationsAsync(citationList);
        if (count > 0)
        {
            _appState.NotifyCitationsChanged();
        }
        return count;
    }

    public async Task<List<string>> GetAllTagsAsync()
    {
        var citations = await _storage.GetAllCitationsAsync();
        return citations
            .SelectMany(c => c.Tags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();
    }

    public async Task<Dictionary<int, int>> GetYearDistributionAsync()
    {
        var citations = await _storage.GetAllCitationsAsync();
        return citations
            .Where(c => c.Year.HasValue)
            .GroupBy(c => c.Year!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
