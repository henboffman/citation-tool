"""
Citation Library - main interface for working with citation data.
"""

import json
from pathlib import Path
from typing import List, Optional, Dict, Any, Union
from collections import Counter
from .models import Citation, Domain, CitationType


class CitationLibrary:
    """
    A library for managing and querying citation data.

    This class provides a high-level interface for working with citations
    exported from the Citation Tool web application.

    Example:
        >>> library = CitationLibrary("citations.json")
        >>> library.search("machine learning", year_from=2020)
        [Citation(...), Citation(...)]
    """

    def __init__(self, data_path: Union[str, Path]):
        """
        Initialize the library from a JSON export file.

        Args:
            data_path: Path to the JSON export file from Citation Tool
        """
        self.data_path = Path(data_path)
        self._citations: List[Citation] = []
        self._domains: List[Domain] = []
        self._load_data()

    def _load_data(self) -> None:
        """Load data from the JSON file."""
        if not self.data_path.exists():
            raise FileNotFoundError(f"Data file not found: {self.data_path}")

        with open(self.data_path, "r", encoding="utf-8") as f:
            data = json.load(f)

        self._citations = [Citation.from_dict(c) for c in data.get("citations", [])]
        self._domains = [Domain.from_dict(d) for d in data.get("domains", [])]

    def reload(self) -> None:
        """Reload data from the file."""
        self._load_data()

    @property
    def citations(self) -> List[Citation]:
        """Get all citations."""
        return self._citations

    @property
    def domains(self) -> List[Domain]:
        """Get all domains."""
        return self._domains

    def __len__(self) -> int:
        """Return the number of citations."""
        return len(self._citations)

    def __iter__(self):
        """Iterate over citations."""
        return iter(self._citations)

    def get_citation(self, citation_id: str) -> Optional[Citation]:
        """
        Get a citation by its ID.

        Args:
            citation_id: The citation's unique ID

        Returns:
            The Citation if found, None otherwise
        """
        for citation in self._citations:
            if citation.id == citation_id:
                return citation
        return None

    def get_domain(self, domain_id: str) -> Optional[Domain]:
        """
        Get a domain by its ID.

        Args:
            domain_id: The domain's unique ID

        Returns:
            The Domain if found, None otherwise
        """
        for domain in self._domains:
            if domain.id == domain_id:
                return domain
        return None

    def get_domain_by_name(self, name: str) -> Optional[Domain]:
        """
        Get a domain by its name (case-insensitive).

        Args:
            name: The domain name

        Returns:
            The Domain if found, None otherwise
        """
        name_lower = name.lower()
        for domain in self._domains:
            if domain.name.lower() == name_lower:
                return domain
        return None

    def search(
        self,
        query: Optional[str] = None,
        domain: Optional[str] = None,
        citation_type: Optional[CitationType] = None,
        year_from: Optional[int] = None,
        year_to: Optional[int] = None,
        tags: Optional[List[str]] = None,
        limit: Optional[int] = None
    ) -> List[Citation]:
        """
        Search citations with various filters.

        Args:
            query: Text to search in title, authors, abstract, notes, tags, DOI
            domain: Domain ID or name to filter by
            citation_type: Filter by citation type
            year_from: Minimum publication year
            year_to: Maximum publication year
            tags: List of tags to filter by (all must match)
            limit: Maximum number of results

        Returns:
            List of matching citations
        """
        results = list(self._citations)

        # Text search
        if query:
            query_lower = query.lower()
            terms = query_lower.split()
            results = [
                c for c in results
                if all(self._matches_term(c, term) for term in terms)
            ]

        # Domain filter
        if domain:
            # Try as ID first, then as name
            domain_obj = self.get_domain(domain) or self.get_domain_by_name(domain)
            if domain_obj:
                results = [c for c in results if c.domain_id == domain_obj.id]
            else:
                results = []

        # Type filter
        if citation_type:
            results = [c for c in results if c.type == citation_type]

        # Year range
        if year_from is not None:
            results = [c for c in results if c.year and c.year >= year_from]
        if year_to is not None:
            results = [c for c in results if c.year and c.year <= year_to]

        # Tags filter
        if tags:
            tags_lower = [t.lower() for t in tags]
            results = [
                c for c in results
                if all(any(t.lower() == tag for t in c.tags) for tag in tags_lower)
            ]

        # Limit
        if limit:
            results = results[:limit]

        return results

    def _matches_term(self, citation: Citation, term: str) -> bool:
        """Check if a citation matches a search term."""
        return (
            term in citation.title.lower() or
            any(term in author.lower() for author in citation.authors) or
            (citation.abstract and term in citation.abstract.lower()) or
            (citation.notes and term in citation.notes.lower()) or
            any(term in tag.lower() for tag in citation.tags) or
            (citation.doi and term in citation.doi.lower())
        )

    def get_by_domain(self, domain: str) -> List[Citation]:
        """
        Get all citations in a specific domain.

        Args:
            domain: Domain ID or name

        Returns:
            List of citations in the domain
        """
        return self.search(domain=domain)

    def get_by_type(self, citation_type: CitationType) -> List[Citation]:
        """
        Get all citations of a specific type.

        Args:
            citation_type: The citation type to filter by

        Returns:
            List of citations of the specified type
        """
        return self.search(citation_type=citation_type)

    def get_by_year(self, year: int) -> List[Citation]:
        """
        Get all citations from a specific year.

        Args:
            year: The publication year

        Returns:
            List of citations from that year
        """
        return self.search(year_from=year, year_to=year)

    def get_tags(self) -> List[str]:
        """
        Get all unique tags sorted alphabetically.

        Returns:
            List of unique tag names
        """
        tags = set()
        for citation in self._citations:
            tags.update(citation.tags)
        return sorted(tags)

    def get_statistics(self) -> Dict[str, Any]:
        """
        Get statistics about the citation library.

        Returns:
            Dictionary with various statistics
        """
        years = [c.year for c in self._citations if c.year]
        type_counts = Counter(c.type.value for c in self._citations)
        domain_counts = Counter(c.domain_id for c in self._citations if c.domain_id)

        return {
            "total_citations": len(self._citations),
            "total_domains": len(self._domains),
            "total_tags": len(self.get_tags()),
            "year_range": {
                "min": min(years) if years else None,
                "max": max(years) if years else None
            },
            "by_type": dict(type_counts),
            "by_domain": {
                self.get_domain(did).name if self.get_domain(did) else did: count
                for did, count in domain_counts.items()
            }
        }

    def to_dataframe(self):
        """
        Convert citations to a pandas DataFrame.

        Returns:
            pandas.DataFrame with citation data

        Raises:
            ImportError: If pandas is not installed
        """
        try:
            import pandas as pd
        except ImportError:
            raise ImportError("pandas is required for to_dataframe(). Install with: pip install pandas")

        data = []
        for c in self._citations:
            domain = self.get_domain(c.domain_id) if c.domain_id else None
            data.append({
                "id": c.id,
                "title": c.title,
                "authors": "; ".join(c.authors),
                "author_count": len(c.authors),
                "type": c.type.value,
                "type_display": c.type.display_name,
                "year": c.year,
                "journal_or_conference": c.journal_or_conference,
                "volume": c.volume,
                "issue": c.issue,
                "pages": c.pages,
                "doi": c.doi,
                "url": c.url,
                "domain_id": c.domain_id,
                "domain_name": domain.name if domain else None,
                "tags": "; ".join(c.tags),
                "tag_count": len(c.tags),
                "has_abstract": c.abstract is not None and len(c.abstract) > 0,
                "abstract_length": len(c.abstract) if c.abstract else 0,
                "date_added": c.date_added,
            })

        return pd.DataFrame(data)

    def export_bibtex(self, citations: Optional[List[Citation]] = None) -> str:
        """
        Export citations to BibTeX format.

        Args:
            citations: List of citations to export (defaults to all)

        Returns:
            BibTeX formatted string
        """
        citations = citations or self._citations
        return "\n\n".join(c.format_bibtex() for c in citations)

    def export_json(self, citations: Optional[List[Citation]] = None, indent: int = 2) -> str:
        """
        Export citations to JSON format.

        Args:
            citations: List of citations to export (defaults to all)
            indent: JSON indentation level

        Returns:
            JSON formatted string
        """
        citations = citations or self._citations
        return json.dumps([c.to_dict() for c in citations], indent=indent)
