using Quickie.DataHandlers.Editonly.Definition;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.EditOnly;

public interface IEditOnlyTodoDataHandler : IEditOnlyDataHandler<PastTodo_EditOnly, string>
{
    Task<ICollection<PastTodo_EditOnly>> GetAllAsync();
}