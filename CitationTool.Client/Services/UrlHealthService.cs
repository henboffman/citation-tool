using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public class UrlHealthService : IUrlHealthService
{
    private readonly HttpClient _httpClient;

    public UrlHealthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<UrlHealthStatus> CheckUrlAsync(string url)
    {
        var status = new UrlHealthStatus
        {
            CheckedAt = DateTime.UtcNow
        };

        if (string.IsNullOrWhiteSpace(url))
        {
            status.IsHealthy = false;
            status.ErrorMessage = "URL is empty";
            return status;
        }

        try
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                status.IsHealthy = false;
                status.ErrorMessage = "Invalid URL format";
                return status;
            }

            using var request = new HttpRequestMessage(HttpMethod.Head, url);
            request.Headers.Add("User-Agent", "CitationManager/1.0 (Health Check)");

            using var response = await _httpClient.SendAsync(request);

            status.StatusCode = (int)response.StatusCode;
            status.IsHealthy = response.IsSuccessStatusCode;

            if (!response.IsSuccessStatusCode)
            {
                status.ErrorMessage = response.ReasonPhrase;
            }
        }
        catch (HttpRequestException ex)
        {
            status.IsHealthy = false;
            status.StatusCode = 0;
            status.ErrorMessage = ex.Message.Contains("SSL")
                ? "SSL/TLS certificate error"
                : ex.Message.Contains("name")
                    ? "Domain not found"
                    : "Connection failed";
        }
        catch (TaskCanceledException)
        {
            status.IsHealthy = false;
            status.StatusCode = 0;
            status.ErrorMessage = "Request timed out";
        }
        catch (Exception ex)
        {
            status.IsHealthy = false;
            status.StatusCode = 0;
            status.ErrorMessage = $"Unexpected error: {ex.Message}";
        }

        return status;
    }

    public async Task<Dictionary<Guid, UrlHealthStatus>> CheckAllUrlsAsync(
        IEnumerable<Citation> citations,
        IProgress<int>? progress = null)
    {
        var results = new Dictionary<Guid, UrlHealthStatus>();
        var citationsWithUrls = citations
            .Where(c => !string.IsNullOrEmpty(c.Url))
            .ToList();

        var total = citationsWithUrls.Count;
        var completed = 0;

        // Process in batches to avoid overwhelming the browser
        const int batchSize = 5;
        var batches = citationsWithUrls
            .Select((c, i) => new { Citation = c, Index = i })
            .GroupBy(x => x.Index / batchSize)
            .Select(g => g.Select(x => x.Citation).ToList());

        foreach (var batch in batches)
        {
            var tasks = batch.Select(async citation =>
            {
                var status = await CheckUrlAsync(citation.Url!);
                return (citation.Id, status);
            });

            var batchResults = await Task.WhenAll(tasks);

            foreach (var (id, status) in batchResults)
            {
                results[id] = status;
                completed++;
                progress?.Report((completed * 100) / total);
            }

            // Small delay between batches to be respectful to servers
            await Task.Delay(100);
        }

        return results;
    }
}
