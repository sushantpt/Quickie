namespace Quickie.Configuration.Options;

/// <summary>
/// Global configuration for Quickie library. Must be initialized once during application startup before any Quickie features are used.
/// </summary>
public static class GlobalQuickieConfigData
{
    private static QuickieOptions? _options;
    
    /// <summary>
    /// Configured Quickie options.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static QuickieOptions Options
    { 
        get => _options ?? throw new InvalidOperationException("QuickieOptions has not been initialized.");
        private set => _options = value;
    }
    
    /// <summary>
    /// Initializes Quickie configuration. This method must be called once during application startup.
    /// </summary>
    /// <param name="options">Configuration options</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Initialize(QuickieOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        Options = options;
    }
}