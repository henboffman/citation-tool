#!/usr/bin/env python3
"""
Basic usage example for Citation Tool Python SDK.

This script demonstrates the core functionality of the library.
"""

import os
import sys

# Add parent directory to path for development
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from citation_tool import CitationLibrary, CitationType


def main():
    # Default data path (can be overridden with environment variable)
    data_path = os.environ.get(
        "CITATION_DATA_PATH",
        os.path.expanduser("~/.citation-tool/citations.json")
    )

    print(f"Loading citations from: {data_path}")

    try:
        library = CitationLibrary(data_path)
    except FileNotFoundError:
        print("\nError: Data file not found!")
        print("Please export your citations from the web app first.")
        print("1. Open Citation Tool in your browser")
        print("2. Go to Settings > Data Backup")
        print("3. Click 'Download Backup'")
        print(f"4. Save the file to: {data_path}")
        return

    # Basic statistics
    print(f"\n=== Citation Library Statistics ===")
    print(f"Total citations: {len(library)}")
    print(f"Total domains: {len(library.domains)}")

    stats = library.get_statistics()
    if stats["year_range"]["min"]:
        print(f"Year range: {stats['year_range']['min']} - {stats['year_range']['max']}")

    # List domains
    print(f"\n=== Domains ===")
    for domain in library.domains:
        count = len(library.get_by_domain(domain.name))
        print(f"  {domain.name}: {count} citations")

    # Citation types
    print(f"\n=== Citation Types ===")
    for type_name, count in stats["by_type"].items():
        print(f"  {type_name}: {count}")

    # Search example
    print(f"\n=== Search Example ===")
    query = "machine learning"
    results = library.search(query, limit=5)
    print(f"Search for '{query}': {len(results)} results")
    for citation in results:
        print(f"  - {citation.title} ({citation.year})")
        print(f"    Authors: {citation.authors_display}")

    # Get tags
    print(f"\n=== Tags ===")
    tags = library.get_tags()
    print(f"Total unique tags: {len(tags)}")
    if tags:
        print(f"Sample tags: {', '.join(tags[:10])}")

    # Format examples
    print(f"\n=== Citation Formatting ===")
    if library.citations:
        citation = library.citations[0]
        print(f"Title: {citation.title}")
        print(f"\nIEEE format:")
        print(f"  {citation.format_ieee()}")
        print(f"\nAPA format:")
        print(f"  {citation.format_apa()}")


if __name__ == "__main__":
    main()
