using Quickie.DataHandlers.Readonly.Definition;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.ReadOnly;

public interface ITodoDataHandler : IReadOnlyCollectionDataHandler<TodoEntity, string>;