using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public interface IStorageService
{
    Task<List<Citation>> GetAllCitationsAsync();
    Task<Citation?> GetCitationAsync(Guid id);
    Task<bool> SaveCitationAsync(Citation citation);
    Task<bool> DeleteCitationAsync(Guid id);
    Task<int> BulkAddCitationsAsync(IEnumerable<Citation> citations);

    Task<List<Domain>> GetAllDomainsAsync();
    Task<Domain?> GetDomainAsync(Guid id);
    Task<bool> SaveDomainAsync(Domain domain);
    Task<bool> DeleteDomainAsync(Guid id);

    Task<List<Citation>> GetCitationsByDomainAsync(Guid domainId);
    Task<int> GetCitationCountAsync();
    Task<int> GetDomainCountAsync();

    Task<string> ExportAllDataAsync();
    Task<bool> ImportAllDataAsync(string jsonData);
    Task<bool> ClearAllDataAsync();
    Task<bool> DeleteDatabaseAsync();
}
