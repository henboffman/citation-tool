# Citation Tool

A modern citation management application for IT professionals and researchers. Organize, search, and manage academic and professional citations with a clean web interface and powerful integrations.

## Features

- **Citation Management**: Add, edit, and organize citations with IEEE/ACM-style fields
- **Domain Organization**: Categorize citations by research area (AI, Security, DevOps, etc.)
- **Smart Search**: Full-text search across titles, authors, abstracts, DOIs, and tags
- **Multiple Export Formats**: CSV, JSON, and BibTeX for LaTeX/Overleaf
- **Import Support**: Import from CSV, JSON, or BibTeX files
- **URL Health Checking**: Verify citation URLs are still accessible
- **Offline-First**: Data stored locally in your browser (IndexedDB)
- **MCP Server**: AI/LLM integration via Model Context Protocol
- **Python SDK**: Programmatic access for data scientists

## Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- A modern web browser (Chrome, Firefox, Edge, Safari)

### Running the Web App

```bash
# Clone the repository
git clone https://github.com/yourusername/citation-tool.git
cd citation-tool

# Run the web application
cd CitationTool.Client
dotnet run
```

Open your browser to `https://localhost:5001` (or the URL shown in the terminal).

The app comes pre-seeded with 150+ important IT publications across multiple domains to get you started.

## Project Structure

```
citation-tool/
├── CitationTool.Client/     # Blazor WebAssembly web application
│   ├── Components/          # Reusable UI components
│   ├── Pages/               # Application pages
│   ├── Services/            # Business logic and data access
│   ├── Models/              # Data models
│   └── wwwroot/             # Static assets
├── CitationTool.Shared/     # Shared models library
├── CitationTool.McpServer/  # MCP server for AI integration
├── python/                  # Python SDK for data scientists
│   └── citation_tool/       # Python package
└── scripts/                 # Setup and utility scripts
```

## MCP Server (AI Integration)

The MCP (Model Context Protocol) server allows AI assistants like Claude to interact with your citation library.

### Setup

```bash
# Build and configure the MCP server
chmod +x scripts/setup-mcp.sh
./scripts/setup-mcp.sh
```

### Configure Claude Desktop

Add to your `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "citation-tool": {
      "command": "dotnet",
      "args": ["/path/to/CitationTool.McpServer/bin/Release/net9.0/citation-mcp-server.dll"],
      "env": {
        "CITATION_DATA_PATH": "~/.citation-tool/citations.json"
      }
    }
  }
}
```

Config file locations:
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

### Available MCP Tools

| Tool | Description |
|------|-------------|
| `search_citations` | Search by query, domain, type, year range, or tags |
| `get_citation` | Get full citation details with formatted references |
| `list_citations` | List all citations with summaries |
| `list_domains` | List available domains/categories |
| `get_citations_by_domain` | Get all citations in a domain |
| `format_citation` | Format a citation (IEEE, APA, BibTeX) |
| `get_statistics` | Get database statistics |
| `list_tags` | List all unique tags |

### Example Prompts

With the MCP server connected, you can ask Claude:

- "Search my citations for papers about transformer architectures"
- "Give me the BibTeX entry for the attention is all you need paper"
- "What citations do I have in the Artificial Intelligence domain?"
- "Find all citations from 2023 about large language models"
- "Create a bibliography for my machine learning papers in APA format"

## Python SDK

For data scientists who want to analyze citations programmatically.

### Installation

```bash
cd python

# Basic installation
pip install -e .

# With pandas support
pip install -e ".[pandas]"
```

### Usage

```python
from citation_tool import CitationLibrary

# Load your citations
library = CitationLibrary("~/.citation-tool/citations.json")

# Search
ml_papers = library.search("machine learning", year_from=2020)

# Convert to pandas DataFrame
df = library.to_dataframe()

# Analyze by year
print(df.groupby('year')['id'].count())

# Export to BibTeX
bibtex = library.export_bibtex(ml_papers)
```

See [python/README.md](python/README.md) for full documentation.

## Data Export/Backup

Your citations are stored locally in your browser. To back up or share your data:

1. Go to **Settings** in the web app
2. Click **Download Backup** under Data Backup
3. Save the JSON file

To use with the MCP server or Python SDK, save the file to:
```
~/.citation-tool/citations.json
```

## Development

### Building

```bash
# Build all projects
dotnet build

# Run web app in development mode
cd CitationTool.Client
dotnet watch run

# Run MCP server
cd CitationTool.McpServer
dotnet run
```

### Testing

```bash
# Run .NET tests
dotnet test

# Run Python tests
cd python
pip install -e ".[dev]"
pytest
```

## Citation Types

The application supports IEEE/ACM standard citation types:

| Type | Description |
|------|-------------|
| Article | Journal articles |
| InProceedings | Conference papers |
| Book | Complete books |
| InBook | Book chapters |
| TechReport | Technical reports |
| Website | Web resources |
| Standard | IEEE/ISO standards |
| Patent | Patents |
| Thesis | PhD/Master's theses |
| Manual | Technical manuals |
| Misc | Other publications |

## Pre-Configured Domains

The application includes these research domains:

- Software Engineering
- Artificial Intelligence
- Security
- DevOps & Lifecycle
- LLM & Agents
- Distributed Systems
- Data Engineering
- Human-Computer Interaction
- Cloud Computing
- Networking
- Database Systems
- Programming Languages
- Testing & Quality
- Web Development

You can create custom domains in the app.

## Security

The application follows modern security best practices:

- Content Security Policy (CSP) headers
- No `eval()` usage in JavaScript
- Input validation with FluentValidation
- URL validation (http/https only)
- XSS protection via Blazor's default encoding
- Local-only data storage (no server transmission)

## Browser Support

- Chrome 80+
- Firefox 75+
- Safari 14+
- Edge 80+

Requires JavaScript and IndexedDB support.

## Technology Stack

- **Frontend**: Blazor WebAssembly (.NET 9)
- **Styling**: Bootstrap 5.3 + Bootstrap Icons
- **Storage**: IndexedDB (browser local storage)
- **Validation**: FluentValidation
- **MCP Server**: .NET 9 console application
- **Python SDK**: Python 3.9+ with optional pandas

## License

MIT License - see [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## Acknowledgments

This project includes seed data featuring seminal papers in computer science and IT. These citations are provided for educational and reference purposes.
