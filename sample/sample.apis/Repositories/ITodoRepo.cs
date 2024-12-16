using Quickie.DataHandlers.Crud.Definition;
using sample.apis.Entities;

namespace sample.apis.Repositories;

public interface ITodoRepo : ICrudDataHandler<TodoEntity, int>;