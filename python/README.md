# Citation Tool Python SDK

A Python library for working with Citation Tool data exports. Designed for data scientists and researchers who want to analyze their citation library programmatically.

## Installation

```bash
# Basic installation
pip install -e .

# With pandas support
pip install -e ".[pandas]"

# Full installation (pandas + jupyter)
pip install -e ".[full]"
```

## Quick Start

```python
from citation_tool import CitationLibrary

# Load your citations
library = CitationLibrary("~/path/to/citations.json")

# See how many citations you have
print(f"Total citations: {len(library)}")

# Search for citations
ml_papers = library.search("machine learning", year_from=2020)
for paper in ml_papers[:5]:
    print(f"- {paper.title} ({paper.year})")

# Get statistics
stats = library.get_statistics()
print(f"Year range: {stats['year_range']['min']} - {stats['year_range']['max']}")
```

## Features

### Search and Filter

```python
# Full-text search
results = library.search("transformer architecture")

# Filter by domain
ai_papers = library.get_by_domain("Artificial Intelligence")

# Filter by type
books = library.get_by_type(CitationType.BOOK)

# Combine filters
recent_ai = library.search(
    query="deep learning",
    domain="Artificial Intelligence",
    year_from=2022,
    tags=["neural-networks"]
)
```

### Citation Formatting

```python
citation = library.get_citation("some-id")

# IEEE format
print(citation.format_ieee())

# APA format
print(citation.format_apa())

# BibTeX format
print(citation.format_bibtex())
```

### Export to Pandas DataFrame

```python
# Requires: pip install pandas
df = library.to_dataframe()

# Analyze by year
print(df.groupby('year')['id'].count())

# Analyze by domain
print(df.groupby('domain_name')['id'].count())

# Find most prolific authors
from collections import Counter
authors = []
for author_str in df['authors']:
    authors.extend(author_str.split('; '))
print(Counter(authors).most_common(10))
```

### Export Formats

```python
# Export to BibTeX
bibtex = library.export_bibtex()
with open("references.bib", "w") as f:
    f.write(bibtex)

# Export subset to JSON
ai_papers = library.search(domain="Artificial Intelligence")
json_data = library.export_json(ai_papers)
```

## Data File Format

The library reads JSON files exported from the Citation Tool web application. Export your data from the web app:

1. Open Citation Tool in your browser
2. Go to Settings > Data Backup
3. Click "Download Backup"
4. Save the JSON file

The file format:
```json
{
  "version": "1.0",
  "exportDate": "2024-01-15T10:30:00Z",
  "citations": [...],
  "domains": [...]
}
```

## API Reference

### CitationLibrary

| Method | Description |
|--------|-------------|
| `search(query, domain, citation_type, year_from, year_to, tags, limit)` | Search with filters |
| `get_citation(id)` | Get citation by ID |
| `get_by_domain(domain)` | Get all citations in a domain |
| `get_by_type(citation_type)` | Get all citations of a type |
| `get_by_year(year)` | Get all citations from a year |
| `get_tags()` | Get all unique tags |
| `get_statistics()` | Get library statistics |
| `to_dataframe()` | Convert to pandas DataFrame |
| `export_bibtex(citations)` | Export to BibTeX |
| `export_json(citations)` | Export to JSON |

### Citation

| Property | Description |
|----------|-------------|
| `id` | Unique identifier |
| `title` | Citation title |
| `authors` | List of author names |
| `type` | CitationType enum |
| `year` | Publication year |
| `doi` | Digital Object Identifier |
| `url` | Web URL |
| `abstract` | Abstract text |
| `tags` | List of tags |

| Method | Description |
|--------|-------------|
| `format_ieee()` | Format as IEEE citation |
| `format_apa()` | Format as APA citation |
| `format_bibtex()` | Format as BibTeX entry |

## License

MIT
