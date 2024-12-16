using Quickie.Types.Readonly;

namespace specification.Helpers.Entities;

public class ReadOnlyEntityConcrete : ReadOnlyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}