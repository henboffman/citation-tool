using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public interface IDomainService
{
    Task<List<Domain>> GetAllAsync();
    Task<Domain?> GetByIdAsync(Guid id);
    Task<ServiceResult<Domain>> CreateAsync(Domain domain);
    Task<ServiceResult<Domain>> UpdateAsync(Domain domain);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<Dictionary<Guid, int>> GetCitationCountsAsync();
}
