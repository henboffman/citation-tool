# Contributing to Citation Tool

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing.

## Development Setup

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Python 3.9+](https://www.python.org/downloads/) (for Python SDK development)
- Git

### Getting Started

```bash
# Clone the repository
git clone https://github.com/yourusername/citation-tool.git
cd citation-tool

# Restore dependencies and build
dotnet restore
dotnet build

# Run the web application
cd CitationTool.Client
dotnet run
```

### Project Structure

| Directory | Purpose |
|-----------|---------|
| `CitationTool.Client/` | Blazor WebAssembly web application |
| `CitationTool.Shared/` | Shared models and DTOs |
| `CitationTool.McpServer/` | MCP server for AI integration |
| `python/` | Python SDK |
| `scripts/` | Utility scripts |

## Making Changes

### Branching Strategy

1. Create a feature branch from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. Make your changes with clear, atomic commits

3. Push and create a pull request

### Code Style

- **C#**: Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Python**: Follow [PEP 8](https://pep8.org/)
- Use the `.editorconfig` settings for consistent formatting

### Commit Messages

Use clear, descriptive commit messages:
- `Add citation export to PDF format`
- `Fix search not finding DOI matches`
- `Update Bootstrap to 5.3.3`

### Testing

Before submitting a PR:

```bash
# Build all projects
dotnet build

# Run the web app and verify functionality
cd CitationTool.Client
dotnet run

# For Python SDK changes
cd python
pip install -e ".[dev]"
pytest
```

## Areas for Contribution

### Good First Issues

- Add more citation format outputs (Chicago, MLA)
- Improve accessibility (ARIA labels, keyboard navigation)
- Add unit tests for services
- Enhance Python SDK with more features

### Feature Ideas

- Cloud sync/backup option
- Citation deduplication
- DOI auto-lookup
- PDF metadata extraction
- Reference graph visualization

## Pull Request Process

1. Ensure your code builds without errors
2. Update documentation if needed
3. Add/update tests for new functionality
4. Create a PR with a clear description
5. Respond to review feedback

## Questions?

Feel free to open an issue for questions or discussion.
