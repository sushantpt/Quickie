using Microsoft.EntityFrameworkCore;
using sample.apis.Entities;

namespace sample.apis.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoEntity> TodoEntity { get; set; }
}