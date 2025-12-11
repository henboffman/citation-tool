using CitationTool.McpServer.Mcp;
using CitationTool.McpServer.Services;

// Citation Tool MCP Server
// Provides AI/LLM access to citation data via the Model Context Protocol

var dataPath = Environment.GetEnvironmentVariable("CITATION_DATA_PATH")
    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".citation-tool", "citations.json");

// Ensure directory exists
var dataDir = Path.GetDirectoryName(dataPath);
if (!string.IsNullOrEmpty(dataDir) && !Directory.Exists(dataDir))
{
    Directory.CreateDirectory(dataDir);
}

Console.Error.WriteLine($"[CitationMCP] Data path: {dataPath}");

if (!File.Exists(dataPath))
{
    Console.Error.WriteLine("[CitationMCP] Warning: Data file not found. Export your citations from the web app first.");
    Console.Error.WriteLine("[CitationMCP] Use the Export feature and save the JSON file to the path above.");
}

var dataService = new CitationDataService(dataPath);
var server = new McpServer(dataService);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await server.RunAsync(cts.Token);
