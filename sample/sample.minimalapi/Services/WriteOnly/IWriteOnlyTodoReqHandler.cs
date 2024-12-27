using Quickie.Handlers.Writeonly.Definition;
using sample.minimalapi.Dtos;

namespace sample.minimalapi.Services.WriteOnly;

public interface IWriteOnlyTodoReqHandler : IWriteOnlyRequestHandler<WriteOnlyTodoDto>
{
    Task<ICollection<WriteOnlyTodoDto>> GetAllAsync();
}