"""
Citation Tool Python SDK

A Python library for working with Citation Tool data exports.
Designed for data scientists and researchers who want to analyze
their citation library programmatically.

Example usage:
    from citation_tool import CitationLibrary

    library = CitationLibrary("citations.json")

    # Search citations
    results = library.search("machine learning")

    # Get statistics
    stats = library.get_statistics()

    # Export to pandas DataFrame
    df = library.to_dataframe()
"""

from .library import CitationLibrary
from .models import Citation, Domain, CitationType

__version__ = "1.0.0"
__all__ = ["CitationLibrary", "Citation", "Domain", "CitationType"]
