using Quickie.Handlers.Crud.Definition;
using sample.minimalapi.Dtos;

namespace sample.minimalapi.Services.Crud;

public interface ITodoService : ICrudRequestHandler<TodoCrudableDto, int>;