using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public interface IUrlHealthService
{
    Task<UrlHealthStatus> CheckUrlAsync(string url);
    Task<Dictionary<Guid, UrlHealthStatus>> CheckAllUrlsAsync(IEnumerable<Citation> citations, IProgress<int>? progress = null);
}
