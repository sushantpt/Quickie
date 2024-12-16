using Quickie.Apis.Readonly;
using sample2.Todos.Dtos;
using sample2.Todos.Services;
using sample2.Todos.Services.ReadOnly;

namespace sample2.Todos.Controllers;

public class ReadSingleTodoController(ISingleTodoReqHandler requestHandler) : ReadOnlyController<TodoDto, ISingleTodoReqHandler, string>(requestHandler);