using FluentValidation;
using CitationTool.Client.Models;

namespace CitationTool.Client.Validators;

public class DomainValidator : AbstractValidator<Domain>
{
    public DomainValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Domain name is required. Enter a name like 'Cloud Computing' or 'Machine Learning'.")
            .MaximumLength(100)
            .WithMessage("Domain name cannot exceed 100 characters.")
            .Matches(@"^[\w\s\-&]+$")
            .WithMessage("Domain name can only contain letters, numbers, spaces, hyphens, and ampersands.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Color is required. Select a color from the palette.")
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid hex color code (e.g., #0d6efd).");
    }
}
