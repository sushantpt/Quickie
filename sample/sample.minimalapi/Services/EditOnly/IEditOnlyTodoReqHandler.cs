using Quickie.Handlers.Editonly.Definition;
using sample.minimalapi.Dtos;

namespace sample.minimalapi.Services.EditOnly;

public interface IEditOnlyTodoReqHandler : IEditOnlyRequestHandler<PastTodo_EditOnlyDto, string>
{
    Task<ICollection<PastTodo_EditOnlyDto>> GetAllAsync();
}