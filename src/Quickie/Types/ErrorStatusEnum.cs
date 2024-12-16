namespace Quickie.Types;

/// <summary>
/// Status representing the different severity levels of an error or message.
/// </summary>
public enum ErrorStatusEnum
{
    /// <summary>
    /// Represents informational messages that do not indicate any issues. These are typically non-critical and meant for logging or user information.
    /// </summary>
    Info = 0,
    
    /// <summary>
    /// Represents warning messages that indicate potential issues. These require attention but do not block the operation or indicate a failure.
    /// </summary>
    Warning = 1,
    
    /// <summary>
    /// Represents critical errors that require immediate attention. These typically indicate a failure or something that must be fixed for the application to function properly.
    /// </summary>
    Critical = 2,
}