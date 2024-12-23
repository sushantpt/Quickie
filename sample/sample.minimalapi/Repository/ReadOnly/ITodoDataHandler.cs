using Quickie.DataHandlers.Readonly.Definition;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.ReadOnly;

public interface ITodoDataHandler : IReadOnlyCollectionDataHandler<TodoEntity, string>;