using Microsoft.EntityFrameworkCore;
using sample.idempotent.redis.Entities;

namespace sample.idempotent.redis;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoCrudableEntity> TodoCrudableEntity { get; set; }
}