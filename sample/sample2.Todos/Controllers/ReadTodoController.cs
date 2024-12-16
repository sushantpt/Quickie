using Quickie.Apis.Readonly;
using sample2.Todos.Dtos;
using sample2.Todos.Services.ReadOnly;

namespace sample2.Todos.Controllers;

public class ReadTodoController(IReadTodoReqHandler requestHandler) : ReadOnlyCollectionController<TodoDto, IReadTodoReqHandler, DataFilterRequest, string>(requestHandler);