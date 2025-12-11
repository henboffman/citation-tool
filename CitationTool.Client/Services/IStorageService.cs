using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public interface IStorageService
{
    // Citations
    Task<List<Citation>> GetAllCitationsAsync();
    Task<Citation?> GetCitationAsync(Guid id);
    Task<bool> SaveCitationAsync(Citation citation);
    Task<bool> DeleteCitationAsync(Guid id);
    Task<int> BulkAddCitationsAsync(IEnumerable<Citation> citations);

    // Domains
    Task<List<Domain>> GetAllDomainsAsync();
    Task<Domain?> GetDomainAsync(Guid id);
    Task<bool> SaveDomainAsync(Domain domain);
    Task<bool> DeleteDomainAsync(Guid id);

    // Saved Searches
    Task<List<SavedSearch>> GetAllSavedSearchesAsync();
    Task<SavedSearch?> GetSavedSearchAsync(Guid id);
    Task<bool> SaveSavedSearchAsync(SavedSearch savedSearch);
    Task<bool> DeleteSavedSearchAsync(Guid id);

    // Queries
    Task<List<Citation>> GetCitationsByDomainAsync(Guid domainId);
    Task<int> GetCitationCountAsync();
    Task<int> GetDomainCountAsync();

    // Data Management
    Task<string> ExportAllDataAsync();
    Task<bool> ImportAllDataAsync(string jsonData);
    Task<bool> ClearAllDataAsync();
    Task<bool> DeleteDatabaseAsync();
}
