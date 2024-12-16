using System.ComponentModel.DataAnnotations;
using Quickie.Types.Crud;

namespace sample.apis.Entities;

public class TodoEntity : CrudEntity
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedDate { get; set; }
}