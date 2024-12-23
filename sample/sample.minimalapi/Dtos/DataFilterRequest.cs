using Quickie.DataObj;

namespace sample.minimalapi.Dtos;

public class DataFilterRequest : RequestForDataObj
{
    public string Name { get; set; } = string.Empty;
}