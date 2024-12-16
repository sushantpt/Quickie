using Quickie.Types;

namespace Quickie.DataObj;

/// <summary>
/// Represents a detailed response object used to return results from an operation.
/// </summary>
public class DetailedResponseObj
{
    /// <summary>
    /// Value indicating whether to show the response.
    /// </summary>
    public bool Show { get; set; } = false;
    
    /// <summary>
    /// Short message that provides high-level information about the response. Can be used like "title" of your detailed error description.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description that explains the response such as troubleshooting information or guidance.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Status of the error or response, indicating the severity of the result.
    /// </summary>
    public ErrorStatusEnum ErrorStatus { get; set; } = ErrorStatusEnum.Info;
}