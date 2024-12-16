using Quickie.DataObj;

namespace specification.Helpers.Dtos;

public class ReadOnlyCollectionDataRequest : RequestForDataObj
{
    public string? FirstName { get; set; }
}