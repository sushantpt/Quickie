using System.ComponentModel.DataAnnotations;
using Quickie.Types.Crud;

namespace sample.idempotent.redis.Entities;

public class TodoCrudableEntity : CrudEntity
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedDate { get; set; }
}