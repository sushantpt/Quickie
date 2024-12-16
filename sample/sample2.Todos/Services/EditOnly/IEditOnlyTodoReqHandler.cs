using Quickie.Handlers.Editonly.Definition;
using sample2.Todos.Dtos;

namespace sample2.Todos.Services.EditOnly;

public interface IEditOnlyTodoReqHandler : IEditOnlyRequestHandler<PastTodo_EditOnlyDto, string>
{
    Task<ICollection<PastTodo_EditOnlyDto>> GetAllAsync();
}