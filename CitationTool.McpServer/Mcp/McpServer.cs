using System.Text.Json;
using CitationTool.McpServer.Services;
using CitationTool.Shared.Models;

namespace CitationTool.McpServer.Mcp;

/// <summary>
/// MCP Server implementation for Citation Tool.
/// Handles JSON-RPC requests over stdio and provides tools/resources for citation access.
/// </summary>
public class McpServer
{
    private readonly CitationDataService _dataService;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _initialized = false;

    public McpServer(CitationDataService dataService)
    {
        _dataService = dataService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Console.Error.WriteLine("[CitationMCP] Server starting...");

        using var reader = new StreamReader(Console.OpenStandardInput());

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null) break;

            if (string.IsNullOrWhiteSpace(line)) continue;

            try
            {
                var request = JsonSerializer.Deserialize<JsonRpcRequest>(line, _jsonOptions);
                if (request != null)
                {
                    var response = await HandleRequestAsync(request);
                    var responseJson = JsonSerializer.Serialize(response, _jsonOptions);
                    Console.WriteLine(responseJson);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CitationMCP] Error processing request: {ex.Message}");
                var errorResponse = new JsonRpcResponse
                {
                    Error = new JsonRpcError
                    {
                        Code = -32700,
                        Message = "Parse error",
                        Data = ex.Message
                    }
                };
                Console.WriteLine(JsonSerializer.Serialize(errorResponse, _jsonOptions));
            }
        }

        Console.Error.WriteLine("[CitationMCP] Server shutting down...");
    }

    private async Task<JsonRpcResponse> HandleRequestAsync(JsonRpcRequest request)
    {
        Console.Error.WriteLine($"[CitationMCP] Received: {request.Method}");

        return request.Method switch
        {
            "initialize" => HandleInitialize(request),
            "initialized" => HandleInitialized(request),
            "tools/list" => HandleToolsList(request),
            "tools/call" => await HandleToolsCallAsync(request),
            "resources/list" => HandleResourcesList(request),
            "resources/read" => await HandleResourcesReadAsync(request),
            "prompts/list" => HandlePromptsList(request),
            "prompts/get" => await HandlePromptsGetAsync(request),
            "ping" => new JsonRpcResponse { Id = request.Id, Result = new { } },
            _ => new JsonRpcResponse
            {
                Id = request.Id,
                Error = new JsonRpcError { Code = -32601, Message = $"Method not found: {request.Method}" }
            }
        };
    }

    private JsonRpcResponse HandleInitialize(JsonRpcRequest request)
    {
        _initialized = true;

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = new InitializeResult
            {
                ProtocolVersion = "2024-11-05",
                ServerInfo = new ServerInfo
                {
                    Name = "citation-tool",
                    Version = "1.0.0"
                },
                Capabilities = new ServerCapabilities
                {
                    Tools = new ToolsCapability { ListChanged = false },
                    Resources = new ResourcesCapability { Subscribe = false, ListChanged = false },
                    Prompts = new PromptsCapability { ListChanged = false }
                }
            }
        };
    }

    private JsonRpcResponse HandleInitialized(JsonRpcRequest request)
    {
        return new JsonRpcResponse { Id = request.Id, Result = new { } };
    }

    private JsonRpcResponse HandleToolsList(JsonRpcRequest request)
    {
        var tools = new List<Tool>
        {
            CreateTool("search_citations",
                "Search citations by query, domain, type, year range, or tags. Returns matching citations with full details.",
                new
                {
                    type = "object",
                    properties = new
                    {
                        query = new { type = "string", description = "Search text to match against title, authors, abstract, notes, tags, and DOI" },
                        domain_id = new { type = "string", description = "Filter by domain ID or domain name" },
                        type = new { type = "string", description = "Filter by citation type (Article, Book, InProceedings, etc.)" },
                        year_from = new { type = "integer", description = "Minimum publication year" },
                        year_to = new { type = "integer", description = "Maximum publication year" },
                        tags = new { type = "array", items = new { type = "string" }, description = "Filter by tags (all must match)" },
                        limit = new { type = "integer", description = "Maximum results to return (default: 50)" }
                    }
                }),

            CreateTool("get_citation",
                "Get a single citation by its ID with full details including formatted citations in multiple styles.",
                new
                {
                    type = "object",
                    properties = new
                    {
                        id = new { type = "string", description = "The citation ID (GUID)" }
                    },
                    required = new[] { "id" }
                }),

            CreateTool("list_citations",
                "List all citations, optionally filtered. Returns a summary of each citation.",
                new
                {
                    type = "object",
                    properties = new
                    {
                        limit = new { type = "integer", description = "Maximum results to return (default: 100)" }
                    }
                }),

            CreateTool("list_domains",
                "List all available domains/categories for citations.",
                new { type = "object", properties = new { } }),

            CreateTool("get_citations_by_domain",
                "Get all citations in a specific domain.",
                new
                {
                    type = "object",
                    properties = new
                    {
                        domain = new { type = "string", description = "Domain ID or name" }
                    },
                    required = new[] { "domain" }
                }),

            CreateTool("format_citation",
                "Format a citation in a specific style (IEEE, APA, BibTeX).",
                new
                {
                    type = "object",
                    properties = new
                    {
                        id = new { type = "string", description = "The citation ID (GUID)" },
                        style = new { type = "string", description = "Citation style: ieee, apa, or bibtex", @enum = new[] { "ieee", "apa", "bibtex" } }
                    },
                    required = new[] { "id", "style" }
                }),

            CreateTool("get_statistics",
                "Get statistics about the citation database.",
                new { type = "object", properties = new { } }),

            CreateTool("list_tags",
                "List all unique tags used across citations.",
                new { type = "object", properties = new { } })
        };

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = new ToolsListResult { Tools = tools }
        };
    }

    private Tool CreateTool(string name, string description, object inputSchema)
    {
        return new Tool
        {
            Name = name,
            Description = description,
            InputSchema = JsonSerializer.SerializeToElement(inputSchema, _jsonOptions)
        };
    }

    private async Task<JsonRpcResponse> HandleToolsCallAsync(JsonRpcRequest request)
    {
        if (!request.Params.HasValue)
        {
            return CreateError(request.Id, -32602, "Missing params");
        }

        var toolName = request.Params.Value.GetProperty("name").GetString();
        var args = request.Params.Value.TryGetProperty("arguments", out var argsElement)
            ? argsElement
            : JsonSerializer.SerializeToElement(new { });

        try
        {
            var result = toolName switch
            {
                "search_citations" => await HandleSearchCitations(args),
                "get_citation" => await HandleGetCitation(args),
                "list_citations" => await HandleListCitations(args),
                "list_domains" => await HandleListDomains(),
                "get_citations_by_domain" => await HandleGetCitationsByDomain(args),
                "format_citation" => await HandleFormatCitation(args),
                "get_statistics" => await HandleGetStatistics(),
                "list_tags" => await HandleListTags(),
                _ => CreateToolError($"Unknown tool: {toolName}")
            };

            return new JsonRpcResponse { Id = request.Id, Result = result };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[CitationMCP] Tool error: {ex.Message}");
            return new JsonRpcResponse
            {
                Id = request.Id,
                Result = CreateToolError(ex.Message)
            };
        }
    }

    private async Task<ToolCallResult> HandleSearchCitations(JsonElement args)
    {
        var query = GetStringArg(args, "query");
        var domainId = GetStringArg(args, "domain_id");
        var type = GetStringArg(args, "type");
        var yearFrom = GetIntArg(args, "year_from");
        var yearTo = GetIntArg(args, "year_to");
        var limit = GetIntArg(args, "limit") ?? 50;
        var tags = GetStringArrayArg(args, "tags");

        var results = await _dataService.SearchCitationsAsync(query, domainId, type, yearFrom, yearTo, tags, limit);
        var domains = await _dataService.GetAllDomainsAsync();

        var output = results.Select(c => FormatCitationSummary(c, domains)).ToList();

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"Found {results.Count} citations:\n\n{string.Join("\n\n", output)}" }
            }
        };
    }

    private async Task<ToolCallResult> HandleGetCitation(JsonElement args)
    {
        var id = GetStringArg(args, "id");
        if (string.IsNullOrEmpty(id))
            return CreateToolError("Missing required parameter: id");

        var citation = await _dataService.GetCitationByIdAsync(id);
        if (citation == null)
            return CreateToolError($"Citation not found: {id}");

        var domains = await _dataService.GetAllDomainsAsync();
        var domain = domains.FirstOrDefault(d => d.Id == citation.DomainId);

        var details = new List<string>
        {
            $"# {citation.Title}",
            "",
            $"**Authors:** {citation.AuthorsDisplay}",
            $"**Type:** {citation.Type.GetDisplayName()}",
            citation.Year.HasValue ? $"**Year:** {citation.Year}" : null,
            domain != null ? $"**Domain:** {domain.Name}" : null,
            !string.IsNullOrEmpty(citation.JournalOrConference) ? $"**Venue:** {citation.JournalOrConference}" : null,
            !string.IsNullOrEmpty(citation.Doi) ? $"**DOI:** {citation.Doi}" : null,
            !string.IsNullOrEmpty(citation.Url) ? $"**URL:** {citation.Url}" : null,
            citation.Tags.Count > 0 ? $"**Tags:** {string.Join(", ", citation.Tags)}" : null,
            "",
            !string.IsNullOrEmpty(citation.Abstract) ? $"**Abstract:**\n{citation.Abstract}" : null,
            !string.IsNullOrEmpty(citation.Notes) ? $"\n**Notes:**\n{citation.Notes}" : null,
            "",
            "## Formatted Citations",
            $"**IEEE:** {citation.FormatIeee()}",
            $"**APA:** {citation.FormatApa()}",
            "",
            $"**BibTeX:**\n```bibtex\n{citation.FormatBibTeX()}\n```"
        };

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = string.Join("\n", details.Where(d => d != null)) }
            }
        };
    }

    private async Task<ToolCallResult> HandleListCitations(JsonElement args)
    {
        var limit = GetIntArg(args, "limit") ?? 100;
        var citations = await _dataService.GetAllCitationsAsync();
        var domains = await _dataService.GetAllDomainsAsync();

        var output = citations.Take(limit).Select(c => FormatCitationSummary(c, domains)).ToList();

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"Total citations: {citations.Count}\n\n{string.Join("\n\n", output)}" }
            }
        };
    }

    private async Task<ToolCallResult> HandleListDomains()
    {
        var domains = await _dataService.GetAllDomainsAsync();
        var citations = await _dataService.GetAllCitationsAsync();

        var output = domains.Select(d =>
        {
            var count = citations.Count(c => c.DomainId == d.Id);
            return $"- **{d.Name}** ({count} citations)\n  ID: {d.Id}\n  {d.Description ?? "No description"}";
        });

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"Available domains ({domains.Count}):\n\n{string.Join("\n\n", output)}" }
            }
        };
    }

    private async Task<ToolCallResult> HandleGetCitationsByDomain(JsonElement args)
    {
        var domain = GetStringArg(args, "domain");
        if (string.IsNullOrEmpty(domain))
            return CreateToolError("Missing required parameter: domain");

        var citations = await _dataService.GetCitationsByDomainAsync(domain);
        var domains = await _dataService.GetAllDomainsAsync();

        if (citations.Count == 0)
            return new ToolCallResult
            {
                Content = new List<ContentBlock>
                {
                    new() { Type = "text", Text = $"No citations found for domain: {domain}" }
                }
            };

        var output = citations.Select(c => FormatCitationSummary(c, domains)).ToList();

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"Found {citations.Count} citations in domain '{domain}':\n\n{string.Join("\n\n", output)}" }
            }
        };
    }

    private async Task<ToolCallResult> HandleFormatCitation(JsonElement args)
    {
        var id = GetStringArg(args, "id");
        var style = GetStringArg(args, "style")?.ToLowerInvariant() ?? "ieee";

        if (string.IsNullOrEmpty(id))
            return CreateToolError("Missing required parameter: id");

        var citation = await _dataService.GetCitationByIdAsync(id);
        if (citation == null)
            return CreateToolError($"Citation not found: {id}");

        var formatted = style switch
        {
            "apa" => citation.FormatApa(),
            "bibtex" => citation.FormatBibTeX(),
            _ => citation.FormatIeee()
        };

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"**{style.ToUpperInvariant()} Format:**\n\n{formatted}" }
            }
        };
    }

    private async Task<ToolCallResult> HandleGetStatistics()
    {
        var stats = await _dataService.GetStatisticsAsync();

        var output = stats.Select(kv => $"- **{kv.Key.Replace("_", " ")}:** {kv.Value}");

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"Citation Database Statistics:\n\n{string.Join("\n", output)}" }
            }
        };
    }

    private async Task<ToolCallResult> HandleListTags()
    {
        var tags = await _dataService.GetAllTagsAsync();

        return new ToolCallResult
        {
            Content = new List<ContentBlock>
            {
                new() { Type = "text", Text = $"Available tags ({tags.Count}):\n\n{string.Join(", ", tags)}" }
            }
        };
    }

    private string FormatCitationSummary(Citation c, List<Domain> domains)
    {
        var domain = domains.FirstOrDefault(d => d.Id == c.DomainId);
        var domainStr = domain != null ? $" [{domain.Name}]" : "";
        var yearStr = c.Year.HasValue ? $" ({c.Year})" : "";

        return $"**{c.Title}**{yearStr}{domainStr}\n" +
               $"  Authors: {c.AuthorsDisplay}\n" +
               $"  Type: {c.Type.GetDisplayName()}\n" +
               $"  ID: {c.Id}";
    }

    private JsonRpcResponse HandleResourcesList(JsonRpcRequest request)
    {
        var resources = new List<Resource>
        {
            new()
            {
                Uri = "citations://all",
                Name = "All Citations",
                Description = "Complete list of all citations in JSON format",
                MimeType = "application/json"
            },
            new()
            {
                Uri = "citations://domains",
                Name = "All Domains",
                Description = "List of all domains/categories",
                MimeType = "application/json"
            },
            new()
            {
                Uri = "citations://statistics",
                Name = "Statistics",
                Description = "Database statistics",
                MimeType = "application/json"
            }
        };

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = new ResourcesListResult { Resources = resources }
        };
    }

    private async Task<JsonRpcResponse> HandleResourcesReadAsync(JsonRpcRequest request)
    {
        if (!request.Params.HasValue)
            return CreateError(request.Id, -32602, "Missing params");

        var uri = request.Params.Value.GetProperty("uri").GetString();

        var content = uri switch
        {
            "citations://all" => JsonSerializer.Serialize(await _dataService.GetAllCitationsAsync(), _jsonOptions),
            "citations://domains" => JsonSerializer.Serialize(await _dataService.GetAllDomainsAsync(), _jsonOptions),
            "citations://statistics" => JsonSerializer.Serialize(await _dataService.GetStatisticsAsync(), _jsonOptions),
            _ => null
        };

        if (content == null)
            return CreateError(request.Id, -32602, $"Unknown resource: {uri}");

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = new ResourceReadResult
            {
                Contents = new List<ResourceContent>
                {
                    new() { Uri = uri!, MimeType = "application/json", Text = content }
                }
            }
        };
    }

    private JsonRpcResponse HandlePromptsList(JsonRpcRequest request)
    {
        var prompts = new List<Prompt>
        {
            new()
            {
                Name = "find_relevant_citations",
                Description = "Find citations relevant to a research topic or question",
                Arguments = new List<PromptArgument>
                {
                    new() { Name = "topic", Description = "The research topic or question", Required = true }
                }
            },
            new()
            {
                Name = "summarize_domain",
                Description = "Summarize all citations in a specific domain",
                Arguments = new List<PromptArgument>
                {
                    new() { Name = "domain", Description = "The domain name", Required = true }
                }
            },
            new()
            {
                Name = "create_bibliography",
                Description = "Create a formatted bibliography for selected citations",
                Arguments = new List<PromptArgument>
                {
                    new() { Name = "query", Description = "Search query to select citations", Required = true },
                    new() { Name = "style", Description = "Citation style (ieee, apa, bibtex)", Required = false }
                }
            }
        };

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = new PromptsListResult { Prompts = prompts }
        };
    }

    private async Task<JsonRpcResponse> HandlePromptsGetAsync(JsonRpcRequest request)
    {
        if (!request.Params.HasValue)
            return CreateError(request.Id, -32602, "Missing params");

        var promptName = request.Params.Value.GetProperty("name").GetString();
        var promptArgs = request.Params.Value.TryGetProperty("arguments", out var argsElement) ? argsElement : default;

        var messages = promptName switch
        {
            "find_relevant_citations" => await CreateFindRelevantPrompt(promptArgs),
            "summarize_domain" => await CreateSummarizeDomainPrompt(promptArgs),
            "create_bibliography" => await CreateBibliographyPrompt(promptArgs),
            _ => new List<PromptMessage>
            {
                new() { Role = "user", Content = new ContentBlock { Text = "Unknown prompt" } }
            }
        };

        return new JsonRpcResponse
        {
            Id = request.Id,
            Result = new GetPromptResult
            {
                Description = $"Prompt: {promptName}",
                Messages = messages
            }
        };
    }

    private async Task<List<PromptMessage>> CreateFindRelevantPrompt(JsonElement args)
    {
        var topic = GetStringArg(args, "topic") ?? "machine learning";
        var citations = await _dataService.SearchCitationsAsync(topic, limit: 20);
        var domains = await _dataService.GetAllDomainsAsync();

        var citationList = string.Join("\n", citations.Select(c => FormatCitationSummary(c, domains)));

        return new List<PromptMessage>
        {
            new()
            {
                Role = "user",
                Content = new ContentBlock
                {
                    Text = $"I'm researching: {topic}\n\nHere are potentially relevant citations from my library:\n\n{citationList}\n\nPlease analyze these citations and recommend the most relevant ones for my research, explaining why each is useful."
                }
            }
        };
    }

    private async Task<List<PromptMessage>> CreateSummarizeDomainPrompt(JsonElement args)
    {
        var domainName = GetStringArg(args, "domain") ?? "Software Engineering";
        var citations = await _dataService.GetCitationsByDomainAsync(domainName);
        var domains = await _dataService.GetAllDomainsAsync();

        var citationList = string.Join("\n", citations.Select(c => FormatCitationSummary(c, domains)));

        return new List<PromptMessage>
        {
            new()
            {
                Role = "user",
                Content = new ContentBlock
                {
                    Text = $"Please summarize the research landscape represented by these {citations.Count} citations in the '{domainName}' domain:\n\n{citationList}\n\nProvide an overview of key themes, seminal works, and trends visible in this collection."
                }
            }
        };
    }

    private async Task<List<PromptMessage>> CreateBibliographyPrompt(JsonElement args)
    {
        var query = GetStringArg(args, "query") ?? "";
        var style = GetStringArg(args, "style") ?? "ieee";
        var citations = await _dataService.SearchCitationsAsync(query, limit: 50);

        var formatted = citations.Select(c => style.ToLowerInvariant() switch
        {
            "apa" => c.FormatApa(),
            "bibtex" => c.FormatBibTeX(),
            _ => c.FormatIeee()
        });

        return new List<PromptMessage>
        {
            new()
            {
                Role = "user",
                Content = new ContentBlock
                {
                    Text = $"Here is a bibliography of {citations.Count} citations in {style.ToUpperInvariant()} format:\n\n{string.Join("\n\n", formatted)}\n\nPlease review this bibliography and suggest any improvements or note any formatting issues."
                }
            }
        };
    }

    // Helper methods
    private static string? GetStringArg(JsonElement args, string name)
    {
        if (args.ValueKind == JsonValueKind.Undefined) return null;
        return args.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String
            ? prop.GetString()
            : null;
    }

    private static int? GetIntArg(JsonElement args, string name)
    {
        if (args.ValueKind == JsonValueKind.Undefined) return null;
        return args.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.Number
            ? prop.GetInt32()
            : null;
    }

    private static List<string>? GetStringArrayArg(JsonElement args, string name)
    {
        if (args.ValueKind == JsonValueKind.Undefined) return null;
        if (!args.TryGetProperty(name, out var prop) || prop.ValueKind != JsonValueKind.Array)
            return null;
        return prop.EnumerateArray().Select(e => e.GetString()!).ToList();
    }

    private static ToolCallResult CreateToolError(string message) => new()
    {
        IsError = true,
        Content = new List<ContentBlock> { new() { Type = "text", Text = $"Error: {message}" } }
    };

    private static JsonRpcResponse CreateError(JsonElement? id, int code, string message) => new()
    {
        Id = id,
        Error = new JsonRpcError { Code = code, Message = message }
    };
}
