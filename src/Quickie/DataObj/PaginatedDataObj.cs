namespace Quickie.DataObj;

/// <summary>
/// Represents an object for paginated data and its metadata.
/// </summary>
public class PaginatedDataObj<TObject> where TObject : class
{
    /// <summary>
    /// Total no. of data.
    /// </summary>
    public int Total { get; set; }
    
    /// <summary>
    /// Collection of data
    /// </summary>
    public IEnumerable<TObject> Items { get; set; } = [];
}