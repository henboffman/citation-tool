using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public interface ICitationService
{
    Task<List<Citation>> GetAllAsync();
    Task<Citation?> GetByIdAsync(Guid id);
    Task<ServiceResult<Citation>> CreateAsync(Citation citation);
    Task<ServiceResult<Citation>> UpdateAsync(Citation citation);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<List<Citation>> SearchAsync(FilterOptions options);
    Task<int> BulkImportAsync(IEnumerable<Citation> citations);
    Task<List<string>> GetAllTagsAsync();
    Task<Dictionary<int, int>> GetYearDistributionAsync();
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> ValidationErrors { get; set; } = new();

    public static ServiceResult Ok() => new() { Success = true };
    public static ServiceResult Fail(string message) => new() { Success = false, ErrorMessage = message };
    public static ServiceResult ValidationFail(IEnumerable<string> errors) => new()
    {
        Success = false,
        ErrorMessage = "Validation failed. Please correct the errors below.",
        ValidationErrors = errors.ToList()
    };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    public new static ServiceResult<T> Fail(string message) => new() { Success = false, ErrorMessage = message };
    public new static ServiceResult<T> ValidationFail(IEnumerable<string> errors) => new()
    {
        Success = false,
        ErrorMessage = "Validation failed. Please correct the errors below.",
        ValidationErrors = errors.ToList()
    };
}
