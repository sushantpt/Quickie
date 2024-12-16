using Quickie.Handlers.Crud.Definition;
using sample.apis.Dtos;

namespace sample.apis.Services;

public interface ITodoService : ICrudRequestHandler<TodoDto, int>;