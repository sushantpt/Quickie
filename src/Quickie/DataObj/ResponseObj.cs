namespace Quickie.DataObj;

/// <summary>
/// Represents a basic response object that contains information about the success of an operation.
/// </summary>
public class ResponseObj
{
    /// <summary>
    /// Value indicating whether the operation was successful. "true" meaning operation has succeeded and "false" meaning operation has failed. 
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Message providing additional information about the operation's result.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Represents a response object that contains the result of an operation along with the data.
/// </summary>
/// <typeparam name="TData">Data.</typeparam>
public class ResponseObj<TData>
{
    /// <summary>
    /// Value indicating whether the operation was successful. "true" meaning operation has succeeded and "false" meaning operation has failed. 
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Message providing additional information about the operation's result.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Data returned as a result of the operation.
    /// </summary>
    public TData Data { get; set; } = default(TData)!;
}