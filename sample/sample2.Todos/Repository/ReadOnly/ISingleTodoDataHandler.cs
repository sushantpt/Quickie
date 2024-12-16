using Quickie.DataHandlers.Readonly.Definition;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.ReadOnly;

public interface ISingleTodoDataHandler : IReadOnlyDataHandler<TodoEntity, string>;