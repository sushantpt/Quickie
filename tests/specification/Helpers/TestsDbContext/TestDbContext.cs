using Microsoft.EntityFrameworkCore;
using specification.Helpers.Entities;

namespace specification.Helpers.TestsDbContext;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<CrudEntityConcrete> CrudEntityConcrete { get; set; }
    public DbSet<EditOnlyEntityConcrete> EditOnlyEntityConcrete { get; set; }
    public DbSet<ReadOnlyEntityConcrete> ReadOnlyEntityConcrete { get; set; }
}