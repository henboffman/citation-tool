using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

/// <summary>
/// Service for managing saved searches (smart lists).
/// </summary>
public interface ISavedSearchService
{
    /// <summary>
    /// Gets all saved searches, ordered by pin status and display order.
    /// </summary>
    Task<List<SavedSearch>> GetAllAsync();

    /// <summary>
    /// Gets a saved search by ID.
    /// </summary>
    Task<SavedSearch?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new saved search.
    /// </summary>
    Task<SavedSearch> CreateAsync(string name, FilterOptions filters, string? description = null);

    /// <summary>
    /// Updates an existing saved search.
    /// </summary>
    Task<bool> UpdateAsync(SavedSearch savedSearch);

    /// <summary>
    /// Deletes a saved search.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Records that a saved search was used (updates LastUsed and UseCount).
    /// </summary>
    Task RecordUsageAsync(Guid id);

    /// <summary>
    /// Toggles the pinned status of a saved search.
    /// </summary>
    Task<bool> TogglePinAsync(Guid id);

    /// <summary>
    /// Reorders saved searches.
    /// </summary>
    Task ReorderAsync(List<Guid> orderedIds);
}
