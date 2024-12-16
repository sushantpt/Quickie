using Quickie.Types.Writable;

namespace specification.Helpers.Entities;

public class WriteOnlyEntityConcrete : WriteOnlyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}