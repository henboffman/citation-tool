using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for managing saved searches (smart lists).
/// </summary>
public class SavedSearchService : ISavedSearchService
{
    private readonly IStorageService _storage;
    private readonly IDomainService _domainService;

    public SavedSearchService(IStorageService storage, IDomainService domainService)
    {
        _storage = storage;
        _domainService = domainService;
    }

    public async Task<List<SavedSearch>> GetAllAsync()
    {
        var searches = await _storage.GetAllSavedSearchesAsync();

        // Order by: pinned first, then by display order, then by last used
        return searches
            .OrderByDescending(s => s.IsPinned)
            .ThenBy(s => s.DisplayOrder)
            .ThenByDescending(s => s.LastUsed ?? s.DateCreated)
            .ToList();
    }

    public async Task<SavedSearch?> GetByIdAsync(Guid id)
    {
        return await _storage.GetSavedSearchAsync(id);
    }

    public async Task<SavedSearch> CreateAsync(string name, FilterOptions filters, string? description = null)
    {
        // Get domain name if domain filter is set
        string? domainName = null;
        if (filters.DomainId.HasValue)
        {
            var domain = await _domainService.GetByIdAsync(filters.DomainId.Value);
            domainName = domain?.Name;
        }

        var savedSearch = new SavedSearch
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            Filters = SavedFilterOptions.FromFilterOptions(filters, domainName),
            DateCreated = DateTime.UtcNow,
            DisplayOrder = await GetNextDisplayOrderAsync()
        };

        await _storage.SaveSavedSearchAsync(savedSearch);
        return savedSearch;
    }

    public async Task<bool> UpdateAsync(SavedSearch savedSearch)
    {
        var existing = await _storage.GetSavedSearchAsync(savedSearch.Id);
        if (existing == null)
            return false;

        return await _storage.SaveSavedSearchAsync(savedSearch);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _storage.DeleteSavedSearchAsync(id);
    }

    public async Task RecordUsageAsync(Guid id)
    {
        var savedSearch = await _storage.GetSavedSearchAsync(id);
        if (savedSearch == null)
            return;

        savedSearch.LastUsed = DateTime.UtcNow;
        savedSearch.UseCount++;

        await _storage.SaveSavedSearchAsync(savedSearch);
    }

    public async Task<bool> TogglePinAsync(Guid id)
    {
        var savedSearch = await _storage.GetSavedSearchAsync(id);
        if (savedSearch == null)
            return false;

        savedSearch.IsPinned = !savedSearch.IsPinned;

        return await _storage.SaveSavedSearchAsync(savedSearch);
    }

    public async Task ReorderAsync(List<Guid> orderedIds)
    {
        var searches = await _storage.GetAllSavedSearchesAsync();
        var searchDict = searches.ToDictionary(s => s.Id);

        for (int i = 0; i < orderedIds.Count; i++)
        {
            if (searchDict.TryGetValue(orderedIds[i], out var search))
            {
                search.DisplayOrder = i;
                await _storage.SaveSavedSearchAsync(search);
            }
        }
    }

    private async Task<int> GetNextDisplayOrderAsync()
    {
        var searches = await _storage.GetAllSavedSearchesAsync();
        return searches.Count > 0
            ? searches.Max(s => s.DisplayOrder) + 1
            : 0;
    }
}
