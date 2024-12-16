using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Writeonly.Implementation;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.WriteOnly;

public class WriteOnlyTodoDataHandler(ApplicationDbContext dbContext) : WriteOnlyDataHandler<WriteOnlyTodoEntity, ApplicationDbContext>(dbContext), IWriteOnlyTodoDataHandler
{
    public async Task<ICollection<WriteOnlyTodoEntity>> GetAllAsync() =>
        await dbContext.Set<WriteOnlyTodoEntity>().ToListAsync();
}