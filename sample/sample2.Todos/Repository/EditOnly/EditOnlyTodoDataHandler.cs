using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Editonly.Implementation;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.EditOnly;

public class EditOnlyTodoDataHandler(DbContext dbContext) : EditOnlyDataHandler<PastTodo_EditOnly, string>(dbContext), IEditOnlyTodoDataHandler
{
    public async Task<ICollection<PastTodo_EditOnly>> GetAllAsync()
    {
        return await dbContext.Set<PastTodo_EditOnly>().AsNoTracking().ToListAsync();
    }
}