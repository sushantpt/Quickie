using Quickie.DataHandlers.Crud.Definition;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.Crud;

public interface ITodoRepo : ICrudDataHandler<TodoCrudableEntity, int>;