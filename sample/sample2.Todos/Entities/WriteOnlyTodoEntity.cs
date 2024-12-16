using System.ComponentModel.DataAnnotations;
using Quickie.Types.Writable;

namespace sample2.Todos.Entities;

public class WriteOnlyTodoEntity : WriteOnlyEntity
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime CreateDt { get; set; }
}