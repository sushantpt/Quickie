using Quickie.Handlers.Crud.Definition;
using sample.idempotent.redis.Dtos;

namespace sample.idempotent.redis.Services.Crud;

public interface ITodoService : ICrudRequestHandler<TodoCrudableDto, int>;