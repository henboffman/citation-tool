using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public interface IImportExportService
{
    Task<ImportResult> ImportCsvAsync(string csvContent);
    Task<ImportResult> ImportJsonAsync(string jsonContent);
    Task<ImportResult> ImportBibTeXAsync(string bibtexContent);

    Task<string> ExportCsvAsync(IEnumerable<Citation> citations);
    Task<string> ExportJsonAsync(IEnumerable<Citation> citations, bool includeAll = true);
    Task<string> ExportBibTeXAsync(IEnumerable<Citation> citations);
}
