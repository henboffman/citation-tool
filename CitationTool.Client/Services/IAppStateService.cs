namespace CitationTool.Client.Services;

/// <summary>
/// Service for notifying components when application state changes.
/// Components can subscribe to events to update their UI when data changes.
/// </summary>
public interface IAppStateService
{
    /// <summary>
    /// Event raised when citations change (added, updated, or deleted).
    /// </summary>
    event Action? OnCitationsChanged;

    /// <summary>
    /// Event raised when domains change (added, updated, or deleted).
    /// </summary>
    event Action? OnDomainsChanged;

    /// <summary>
    /// Notify subscribers that citations have changed.
    /// </summary>
    void NotifyCitationsChanged();

    /// <summary>
    /// Notify subscribers that domains have changed.
    /// </summary>
    void NotifyDomainsChanged();
}

public class AppStateService : IAppStateService
{
    public event Action? OnCitationsChanged;
    public event Action? OnDomainsChanged;

    public void NotifyCitationsChanged()
    {
        OnCitationsChanged?.Invoke();
    }

    public void NotifyDomainsChanged()
    {
        OnDomainsChanged?.Invoke();
    }
}
