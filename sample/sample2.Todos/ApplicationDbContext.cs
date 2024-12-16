using Microsoft.EntityFrameworkCore;
using sample2.Todos.Entities;

namespace sample2.Todos;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoEntity> TodoEntity { get; set; }
    public DbSet<PastTodo_EditOnly> PastTodo_EditOnly { get; set; }
    public DbSet<WriteOnlyTodoEntity> WriteOnlyTodoEntity { get; set; }
}