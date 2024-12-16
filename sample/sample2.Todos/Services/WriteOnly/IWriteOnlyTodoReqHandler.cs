using Quickie.Handlers.Writeonly.Definition;
using sample2.Todos.Dtos;

namespace sample2.Todos.Services.WriteOnly;

public interface IWriteOnlyTodoReqHandler : IWriteOnlyRequestHandler<WriteOnlyTodoDto>
{
    Task<ICollection<WriteOnlyTodoDto>> GetAllAsync();
}