namespace Quickie.DataObj;

/// <summary>
/// Represents a request for data with pagination.
/// </summary>
public class RequestForDataObj
{
    /// <summary>
    /// The number of items to retrieve per page. Default is <c>10</c>.
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// The page number to retrieve. Default is <c>1</c>.
    /// </summary>
    public int PageNumber { get; set; } = 1;
};