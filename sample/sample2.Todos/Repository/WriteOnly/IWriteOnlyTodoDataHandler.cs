using Quickie.DataHandlers.Writeonly.Definition;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.WriteOnly;

public interface IWriteOnlyTodoDataHandler : IWriteOnlyDataHandler<WriteOnlyTodoEntity>
{
    Task<ICollection<WriteOnlyTodoEntity>> GetAllAsync();
}