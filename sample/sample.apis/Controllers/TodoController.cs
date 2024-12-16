using Quickie.Apis.Crud;
using sample.apis.Dtos;
using sample.apis.Services;

namespace sample.apis.Controllers;

public class TodoController(ITodoService requestHandler) : CrudController<TodoDto, ITodoService, int>(requestHandler);