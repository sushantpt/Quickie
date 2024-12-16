using Quickie.DataHandlers.Editonly.Definition;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.EditOnly;

public interface IEditOnlyTodoDataHandler : IEditOnlyDataHandler<PastTodo_EditOnly, string>
{
    Task<ICollection<PastTodo_EditOnly>> GetAllAsync();
}