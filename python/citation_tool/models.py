"""
Data models for Citation Tool.
"""

from dataclasses import dataclass, field
from datetime import datetime
from enum import Enum
from typing import Optional, List
import json


class CitationType(Enum):
    """Citation types matching IEEE/ACM standards."""
    ARTICLE = "Article"
    IN_PROCEEDINGS = "InProceedings"
    BOOK = "Book"
    IN_BOOK = "InBook"
    TECH_REPORT = "TechReport"
    WEBSITE = "Website"
    STANDARD = "Standard"
    PATENT = "Patent"
    THESIS = "Thesis"
    MANUAL = "Manual"
    MISC = "Misc"

    @property
    def display_name(self) -> str:
        """Human-readable name for the citation type."""
        names = {
            CitationType.ARTICLE: "Journal Article",
            CitationType.IN_PROCEEDINGS: "Conference Paper",
            CitationType.BOOK: "Book",
            CitationType.IN_BOOK: "Book Chapter",
            CitationType.TECH_REPORT: "Technical Report",
            CitationType.WEBSITE: "Website",
            CitationType.STANDARD: "Standard",
            CitationType.PATENT: "Patent",
            CitationType.THESIS: "Thesis",
            CitationType.MANUAL: "Manual",
            CitationType.MISC: "Miscellaneous",
        }
        return names.get(self, self.value)


@dataclass
class Domain:
    """A domain/category for organizing citations."""
    id: str
    name: str
    description: Optional[str] = None
    color: str = "#6c757d"
    date_created: Optional[datetime] = None

    @classmethod
    def from_dict(cls, data: dict) -> "Domain":
        """Create a Domain from a dictionary."""
        return cls(
            id=data.get("id", ""),
            name=data.get("name", ""),
            description=data.get("description"),
            color=data.get("color", "#6c757d"),
            date_created=_parse_datetime(data.get("dateCreated"))
        )


@dataclass
class Citation:
    """A citation/reference entry."""
    id: str
    title: str
    authors: List[str] = field(default_factory=list)
    type: CitationType = CitationType.ARTICLE
    journal_or_conference: Optional[str] = None
    volume: Optional[str] = None
    issue: Optional[str] = None
    pages: Optional[str] = None
    year: Optional[int] = None
    month: Optional[str] = None
    publisher: Optional[str] = None
    doi: Optional[str] = None
    url: Optional[str] = None
    isbn: Optional[str] = None
    abstract: Optional[str] = None
    notes: Optional[str] = None
    tags: List[str] = field(default_factory=list)
    domain_id: Optional[str] = None
    date_added: Optional[datetime] = None
    date_modified: Optional[datetime] = None

    @classmethod
    def from_dict(cls, data: dict) -> "Citation":
        """Create a Citation from a dictionary."""
        # Parse citation type
        type_str = data.get("type", "Article")
        try:
            citation_type = CitationType(type_str)
        except ValueError:
            citation_type = CitationType.MISC

        return cls(
            id=data.get("id", ""),
            title=data.get("title", ""),
            authors=data.get("authors", []),
            type=citation_type,
            journal_or_conference=data.get("journalOrConference"),
            volume=data.get("volume"),
            issue=data.get("issue"),
            pages=data.get("pages"),
            year=data.get("year"),
            month=data.get("month"),
            publisher=data.get("publisher"),
            doi=data.get("doi"),
            url=data.get("url"),
            isbn=data.get("isbn"),
            abstract=data.get("abstract"),
            notes=data.get("notes"),
            tags=data.get("tags", []),
            domain_id=data.get("domainId"),
            date_added=_parse_datetime(data.get("dateAdded")),
            date_modified=_parse_datetime(data.get("dateModified"))
        )

    @property
    def authors_display(self) -> str:
        """Formatted author list."""
        return ", ".join(self.authors) if self.authors else "Unknown Author"

    def format_ieee(self) -> str:
        """Format citation in IEEE style."""
        parts = []

        if self.authors:
            author_str = f"{self.authors[0]} et al." if len(self.authors) > 3 else ", ".join(self.authors)
            parts.append(author_str)

        if self.title:
            parts.append(f'"{self.title}"')

        if self.journal_or_conference:
            prefix = "in " if self.type == CitationType.IN_PROCEEDINGS else ""
            parts.append(f"{prefix}{self.journal_or_conference}")

        if self.volume:
            vol_str = f"vol. {self.volume}, no. {self.issue}" if self.issue else f"vol. {self.volume}"
            parts.append(vol_str)

        if self.pages:
            parts.append(f"pp. {self.pages}")

        if self.year:
            year_str = f"{self.month} {self.year}" if self.month else str(self.year)
            parts.append(year_str)

        if self.doi:
            parts.append(f"doi: {self.doi}")

        return ", ".join(parts) + "."

    def format_apa(self) -> str:
        """Format citation in APA style."""
        parts = []

        if self.authors:
            formatted_authors = []
            for author in self.authors:
                name_parts = author.split()
                if len(name_parts) >= 2:
                    formatted_authors.append(f"{name_parts[-1]}, {' '.join(n[0] + '.' for n in name_parts[:-1])}")
                else:
                    formatted_authors.append(author)
            parts.append(", ".join(formatted_authors))

        if self.year:
            parts.append(f"({self.year})")

        if self.title:
            parts.append(self.title)

        if self.journal_or_conference:
            parts.append(f"*{self.journal_or_conference}*")

        if self.volume:
            vol_str = f"*{self.volume}*({self.issue})" if self.issue else f"*{self.volume}*"
            parts.append(vol_str)

        if self.pages:
            parts.append(self.pages)

        if self.doi:
            parts.append(f"https://doi.org/{self.doi}")

        return ". ".join(filter(None, parts))

    def format_bibtex(self) -> str:
        """Format citation in BibTeX format."""
        # Generate key
        author_key = self.authors[0].split()[-1].lower() if self.authors else "unknown"
        year_key = str(self.year) if self.year else "0000"
        title_key = self.title.split()[0].lower() if self.title else "untitled"
        key = f"{author_key}{year_key}{title_key}"

        # Entry type
        entry_types = {
            CitationType.ARTICLE: "article",
            CitationType.IN_PROCEEDINGS: "inproceedings",
            CitationType.BOOK: "book",
            CitationType.IN_BOOK: "inbook",
            CitationType.TECH_REPORT: "techreport",
            CitationType.THESIS: "phdthesis",
            CitationType.MANUAL: "manual",
        }
        entry_type = entry_types.get(self.type, "misc")

        lines = [f"@{entry_type}{{{key},"]
        lines.append(f"  title = {{{self.title}}}")

        if self.authors:
            lines.append(f"  author = {{{' and '.join(self.authors)}}}")
        if self.year:
            lines.append(f"  year = {{{self.year}}}")
        if self.journal_or_conference:
            field = "booktitle" if self.type == CitationType.IN_PROCEEDINGS else "journal"
            lines.append(f"  {field} = {{{self.journal_or_conference}}}")
        if self.volume:
            lines.append(f"  volume = {{{self.volume}}}")
        if self.issue:
            lines.append(f"  number = {{{self.issue}}}")
        if self.pages:
            lines.append(f"  pages = {{{self.pages}}}")
        if self.publisher:
            lines.append(f"  publisher = {{{self.publisher}}}")
        if self.doi:
            lines.append(f"  doi = {{{self.doi}}}")
        if self.url:
            lines.append(f"  url = {{{self.url}}}")
        if self.isbn:
            lines.append(f"  isbn = {{{self.isbn}}}")

        lines.append("}")

        return ",\n".join(lines[:-1]) + "\n}"

    def to_dict(self) -> dict:
        """Convert to dictionary for JSON serialization."""
        return {
            "id": self.id,
            "title": self.title,
            "authors": self.authors,
            "type": self.type.value,
            "journalOrConference": self.journal_or_conference,
            "volume": self.volume,
            "issue": self.issue,
            "pages": self.pages,
            "year": self.year,
            "month": self.month,
            "publisher": self.publisher,
            "doi": self.doi,
            "url": self.url,
            "isbn": self.isbn,
            "abstract": self.abstract,
            "notes": self.notes,
            "tags": self.tags,
            "domainId": self.domain_id,
        }


def _parse_datetime(value: Optional[str]) -> Optional[datetime]:
    """Parse an ISO datetime string."""
    if not value:
        return None
    try:
        # Handle various ISO formats
        value = value.replace("Z", "+00:00")
        return datetime.fromisoformat(value)
    except (ValueError, TypeError):
        return None
