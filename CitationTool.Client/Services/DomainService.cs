using FluentValidation;
using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public class DomainService : IDomainService
{
    private readonly IStorageService _storage;
    private readonly IValidator<Domain> _validator;

    public DomainService(IStorageService storage, IValidator<Domain> validator)
    {
        _storage = storage;
        _validator = validator;
    }

    public async Task<List<Domain>> GetAllAsync()
    {
        return await _storage.GetAllDomainsAsync();
    }

    public async Task<Domain?> GetByIdAsync(Guid id)
    {
        return await _storage.GetDomainAsync(id);
    }

    public async Task<ServiceResult<Domain>> CreateAsync(Domain domain)
    {
        var validationResult = await _validator.ValidateAsync(domain);
        if (!validationResult.IsValid)
        {
            return ServiceResult<Domain>.ValidationFail(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        // Check for duplicate name
        var existing = await _storage.GetAllDomainsAsync();
        if (existing.Any(d => d.Name.Equals(domain.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return ServiceResult<Domain>.Fail(
                "A domain with this name already exists. Please choose a different name.");
        }

        domain.Id = Guid.NewGuid();
        domain.DateCreated = DateTime.UtcNow;

        var success = await _storage.SaveDomainAsync(domain);
        if (!success)
        {
            return ServiceResult<Domain>.Fail(
                "Unable to save domain. Please try again. If the problem persists, try refreshing the page.");
        }

        return ServiceResult<Domain>.Ok(domain);
    }

    public async Task<ServiceResult<Domain>> UpdateAsync(Domain domain)
    {
        var validationResult = await _validator.ValidateAsync(domain);
        if (!validationResult.IsValid)
        {
            return ServiceResult<Domain>.ValidationFail(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var existing = await _storage.GetDomainAsync(domain.Id);
        if (existing == null)
        {
            return ServiceResult<Domain>.Fail(
                "Domain not found. It may have been deleted. Please refresh and try again.");
        }

        // Check for duplicate name (excluding current domain)
        var allDomains = await _storage.GetAllDomainsAsync();
        if (allDomains.Any(d => d.Id != domain.Id &&
            d.Name.Equals(domain.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return ServiceResult<Domain>.Fail(
                "A domain with this name already exists. Please choose a different name.");
        }

        var success = await _storage.SaveDomainAsync(domain);
        if (!success)
        {
            return ServiceResult<Domain>.Fail(
                "Unable to update domain. Please try again. If the problem persists, try refreshing the page.");
        }

        return ServiceResult<Domain>.Ok(domain);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var existing = await _storage.GetDomainAsync(id);
        if (existing == null)
        {
            return ServiceResult.Fail(
                "Domain not found. It may have already been deleted.");
        }

        // Check if domain has citations
        var citations = await _storage.GetCitationsByDomainAsync(id);
        if (citations.Count > 0)
        {
            return ServiceResult.Fail(
                $"Cannot delete domain. {citations.Count} citation(s) are assigned to this domain. " +
                "Please reassign or delete those citations first.");
        }

        var success = await _storage.DeleteDomainAsync(id);
        if (!success)
        {
            return ServiceResult.Fail(
                "Unable to delete domain. Please try again. If the problem persists, try refreshing the page.");
        }

        return ServiceResult.Ok();
    }

    public async Task<Dictionary<Guid, int>> GetCitationCountsAsync()
    {
        var citations = await _storage.GetAllCitationsAsync();
        return citations
            .Where(c => c.DomainId.HasValue)
            .GroupBy(c => c.DomainId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
