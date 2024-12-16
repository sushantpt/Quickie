using Quickie.Types.Editonly;

namespace specification.Helpers.Entities;

public class EditOnlyEntityConcrete : EditOnlyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}