using Quickie.Handlers.Readonly.Definition;
using sample2.Todos.Dtos;

namespace sample2.Todos.Services.ReadOnly;

public interface IReadTodoReqHandler : IReadOnlyCollectionRequestHandler<TodoDto, string>;