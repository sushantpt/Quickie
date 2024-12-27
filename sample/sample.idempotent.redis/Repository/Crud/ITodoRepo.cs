using Quickie.DataHandlers.Crud.Definition;
using sample.idempotent.redis.Entities;

namespace sample.idempotent.redis.Repository.Crud;

public interface ITodoRepo : ICrudDataHandler<TodoCrudableEntity, int>;