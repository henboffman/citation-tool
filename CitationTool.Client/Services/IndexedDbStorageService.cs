using System.Text.Json;
using Microsoft.JSInterop;
using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public class IndexedDbStorageService : IStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerOptions _jsonOptions;

    public IndexedDbStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<List<Citation>> GetAllCitationsAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>("indexedDbInterop.getAll", "citations");
            return DeserializeList<Citation>(result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting citations: {ex.Message}");
            return new List<Citation>();
        }
    }

    public async Task<Citation?> GetCitationAsync(Guid id)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement?>("indexedDbInterop.get", "citations", id.ToString());
            if (result == null || result.Value.ValueKind == JsonValueKind.Null)
                return null;
            return JsonSerializer.Deserialize<Citation>(result.Value.GetRawText(), _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting citation {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SaveCitationAsync(Citation citation)
    {
        try
        {
            citation.DateModified = DateTime.UtcNow;
            var jsonElement = JsonSerializer.SerializeToElement(citation, _jsonOptions);
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.put", "citations", jsonElement);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving citation: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCitationAsync(Guid id)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.delete", "citations", id.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting citation {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<int> BulkAddCitationsAsync(IEnumerable<Citation> citations)
    {
        try
        {
            var citationList = citations.ToList();
            foreach (var citation in citationList)
            {
                citation.DateModified = DateTime.UtcNow;
            }
            var jsonElement = JsonSerializer.SerializeToElement(citationList, _jsonOptions);
            return await _jsRuntime.InvokeAsync<int>("indexedDbInterop.bulkAdd", "citations", jsonElement);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error bulk adding citations: {ex.Message}");
            return 0;
        }
    }

    public async Task<List<Domain>> GetAllDomainsAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>("indexedDbInterop.getAll", "domains");
            return DeserializeList<Domain>(result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting domains: {ex.Message}");
            return new List<Domain>();
        }
    }

    public async Task<Domain?> GetDomainAsync(Guid id)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement?>("indexedDbInterop.get", "domains", id.ToString());
            if (result == null || result.Value.ValueKind == JsonValueKind.Null)
                return null;
            return JsonSerializer.Deserialize<Domain>(result.Value.GetRawText(), _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting domain {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SaveDomainAsync(Domain domain)
    {
        try
        {
            var jsonElement = JsonSerializer.SerializeToElement(domain, _jsonOptions);
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.put", "domains", jsonElement);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving domain: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteDomainAsync(Guid id)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.delete", "domains", id.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting domain {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<List<SavedSearch>> GetAllSavedSearchesAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>("indexedDbInterop.getAll", "savedSearches");
            return DeserializeList<SavedSearch>(result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting saved searches: {ex.Message}");
            return new List<SavedSearch>();
        }
    }

    public async Task<SavedSearch?> GetSavedSearchAsync(Guid id)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement?>("indexedDbInterop.get", "savedSearches", id.ToString());
            if (result == null || result.Value.ValueKind == JsonValueKind.Null)
                return null;
            return JsonSerializer.Deserialize<SavedSearch>(result.Value.GetRawText(), _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting saved search {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> SaveSavedSearchAsync(SavedSearch savedSearch)
    {
        try
        {
            var jsonElement = JsonSerializer.SerializeToElement(savedSearch, _jsonOptions);
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.put", "savedSearches", jsonElement);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving saved search: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteSavedSearchAsync(Guid id)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.delete", "savedSearches", id.ToString());
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting saved search {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Citation>> GetCitationsByDomainAsync(Guid domainId)
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>(
                "indexedDbInterop.getByIndex", "citations", "domainId", domainId.ToString());
            return DeserializeList<Citation>(result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error getting citations by domain {domainId}: {ex.Message}");
            return new List<Citation>();
        }
    }

    public async Task<int> GetCitationCountAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<int>("indexedDbInterop.count", "citations");
        }
        catch
        {
            return 0;
        }
    }

    public async Task<int> GetDomainCountAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<int>("indexedDbInterop.count", "domains");
        }
        catch
        {
            return 0;
        }
    }

    public async Task<string> ExportAllDataAsync()
    {
        try
        {
            var result = await _jsRuntime.InvokeAsync<JsonElement>("indexedDbInterop.exportAllData");
            return result.GetRawText();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error exporting data: {ex.Message}");
            return "{}";
        }
    }

    public async Task<bool> ImportAllDataAsync(string jsonData)
    {
        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonData);
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.importAllData", jsonElement);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error importing data: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ClearAllDataAsync()
    {
        try
        {
            await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.clear", "citations");
            await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.clear", "domains");
            await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.clear", "savedSearches");
            await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.clear", "settings");
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error clearing data: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteDatabaseAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<bool>("indexedDbInterop.deleteDatabase");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting database: {ex.Message}");
            return false;
        }
    }

    private List<T> DeserializeList<T>(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<T>>(element.GetRawText(), _jsonOptions) ?? new List<T>();
        }
        return new List<T>();
    }
}
