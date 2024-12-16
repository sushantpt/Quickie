using Quickie.DataObj;

namespace sample2.Todos.Dtos;

public class DataFilterRequest : RequestForDataObj
{
    public string Name { get; set; } = string.Empty;
}