using Quickie.Handlers.Writeonly.Implementation;
using sample2.Todos.Dtos;
using sample2.Todos.Entities;
using sample2.Todos.Repository.WriteOnly;

namespace sample2.Todos.Services.WriteOnly;

public class WriteOnlyTodoReqHandler(IWriteOnlyTodoDataHandler dataHandler) : WriteOnlyRequestHandler<WriteOnlyTodoDto, WriteOnlyTodoEntity, IWriteOnlyTodoDataHandler>(dataHandler), IWriteOnlyTodoReqHandler
{
    protected override WriteOnlyTodoEntity MapToEntity(WriteOnlyTodoDto requestModel) => 
        new()
        {
            Name = requestModel.Name,
            Description = requestModel.Description,
            CreateDt = DateTime.Now
        };

    protected override WriteOnlyTodoDto MapToDto(WriteOnlyTodoEntity entity) => new(Name: entity.Name, Description: entity.Description);

    protected override ICollection<WriteOnlyTodoEntity> MapToCollectionOfEntity(ICollection<WriteOnlyTodoDto> requestModels)
    {
        var data = new List<WriteOnlyTodoEntity>();
        requestModels.ToList().ForEach(x => data.Add(new WriteOnlyTodoEntity
        {
            Name = x.Name,
            Description = x.Description,
            CreateDt = DateTime.Now
        }));
        return data;
    }

    protected override ICollection<WriteOnlyTodoDto> MapToCollectionOfDto(ICollection<WriteOnlyTodoEntity> entities)
    {
        var data = new List<WriteOnlyTodoDto>();
        entities.ToList().ForEach(x => data.Add(new WriteOnlyTodoDto(Name: x.Name, Description: x.Description)));
        return data;
    }

    public async Task<ICollection<WriteOnlyTodoDto>> GetAllAsync()
    {
        var data = await dataHandler.GetAllAsync();
        return MapToCollectionOfDto(data);
    }
}