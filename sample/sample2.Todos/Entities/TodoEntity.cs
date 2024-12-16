using System.ComponentModel.DataAnnotations;
using Quickie.Types.Readonly;

namespace sample2.Todos.Entities;

public class TodoEntity : ReadOnlyEntity
{
    [Key]
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreateDt { get; set; }
}