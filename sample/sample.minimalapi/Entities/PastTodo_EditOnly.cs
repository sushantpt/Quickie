using System.ComponentModel.DataAnnotations;
using Quickie.Types.Editonly;

namespace sample.minimalapi.Entities;

public class PastTodo_EditOnly : EditOnlyEntity
{
    [Key]
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreateDt { get; set; }
}