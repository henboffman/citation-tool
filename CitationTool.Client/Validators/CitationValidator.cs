using FluentValidation;
using CitationTool.Client.Models;

namespace CitationTool.Client.Validators;

public class CitationValidator : AbstractValidator<Citation>
{
    public CitationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required. Enter the title of the publication.")
            .MaximumLength(500)
            .WithMessage("Title cannot exceed 500 characters. Consider shortening or abbreviating.");

        RuleFor(x => x.Authors)
            .NotEmpty()
            .WithMessage("At least one author is required. Enter the author name(s).")
            .Must(authors => authors.All(a => !string.IsNullOrWhiteSpace(a)))
            .WithMessage("Author names cannot be empty. Remove any blank entries.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1500, DateTime.Now.Year + 1)
            .When(x => x.Year.HasValue)
            .WithMessage($"Year must be between 1500 and {DateTime.Now.Year + 1}. Check the publication date.");

        RuleFor(x => x.Doi)
            .Matches(@"^10\.\d{4,}/[^\s]+$")
            .When(x => !string.IsNullOrEmpty(x.Doi))
            .WithMessage("DOI format is invalid. Expected format: 10.xxxx/identifier (e.g., 10.1109/ACCESS.2020.1234567)");

        RuleFor(x => x.Url)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.Url))
            .WithMessage("URL format is invalid. Include the full URL with http:// or https://");

        RuleFor(x => x.Isbn)
            .Matches(@"^(?:\d{10}|\d{13}|(?:\d{1,5}-){3,4}\d{1,7})$")
            .When(x => !string.IsNullOrEmpty(x.Isbn))
            .WithMessage("ISBN format is invalid. Enter a valid 10 or 13 digit ISBN.");

        RuleFor(x => x.Pages)
            .Matches(@"^\d+(-\d+)?$")
            .When(x => !string.IsNullOrEmpty(x.Pages))
            .WithMessage("Pages format is invalid. Use format: 123 or 123-145");

        RuleFor(x => x.JournalOrConference)
            .NotEmpty()
            .When(x => x.Type == CitationType.Article || x.Type == CitationType.InProceedings)
            .WithMessage("Journal/Conference name is required for articles and conference papers.");

        RuleFor(x => x.Abstract)
            .MaximumLength(5000)
            .WithMessage("Abstract cannot exceed 5000 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithMessage("Notes cannot exceed 2000 characters.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50)
            .WithMessage("Individual tags cannot exceed 50 characters.");
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
