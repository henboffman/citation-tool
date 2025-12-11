namespace CitationTool.Client.Models;

public class Domain
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#6c757d";
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}

public static class DomainColors
{
    public static readonly string[] Available = new[]
    {
        "#0d6efd", // Blue
        "#6610f2", // Indigo
        "#6f42c1", // Purple
        "#d63384", // Pink
        "#dc3545", // Red
        "#fd7e14", // Orange
        "#ffc107", // Yellow
        "#198754", // Green
        "#20c997", // Teal
        "#0dcaf0", // Cyan
        "#6c757d", // Gray
        "#343a40"  // Dark
    };
}
