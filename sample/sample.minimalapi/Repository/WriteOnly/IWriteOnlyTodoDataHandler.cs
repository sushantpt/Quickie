using Quickie.DataHandlers.Writeonly.Definition;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.WriteOnly;

public interface IWriteOnlyTodoDataHandler : IWriteOnlyDataHandler<WriteOnlyTodoEntity>
{
    Task<ICollection<WriteOnlyTodoEntity>> GetAllAsync();
}