using System.ComponentModel.DataAnnotations;
using Quickie.Types.Crud;

namespace specification.Helpers.Entities;

public class CrudEntityConcrete : CrudEntity
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}