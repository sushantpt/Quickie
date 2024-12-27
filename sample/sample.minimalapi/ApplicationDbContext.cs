using Microsoft.EntityFrameworkCore;
using sample.minimalapi.Entities;

namespace sample.minimalapi;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoEntity> TodoEntity { get; set; }
    public DbSet<PastTodo_EditOnly> PastTodo_EditOnly { get; set; }
    public DbSet<WriteOnlyTodoEntity> WriteOnlyTodoEntity { get; set; }
    public DbSet<TodoCrudableEntity> TodoCrudableEntity { get; set; }
}