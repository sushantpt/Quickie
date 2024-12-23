using Quickie.Handlers.Editonly.Implementation;
using sample.minimalapi.Dtos;
using sample.minimalapi.Entities;
using sample.minimalapi.Repository.EditOnly;

namespace sample.minimalapi.Services.EditOnly;

public class EditOnlyTodoReqHandler(IEditOnlyTodoDataHandler dataHandler) : EditOnlyRequestHandler<PastTodo_EditOnlyDto,PastTodo_EditOnly,IEditOnlyTodoDataHandler ,string>(dataHandler), IEditOnlyTodoReqHandler
{
    protected override PastTodo_EditOnly MapToEntity(PastTodo_EditOnlyDto requestModel) =>
        new() 
        { 
            Id = requestModel.Id,
            Name = requestModel.Name,
            Description = requestModel.Description,
            CreateDt = requestModel.CreateDt,
        };

    protected override PastTodo_EditOnlyDto MapToDto(PastTodo_EditOnly entity) => 
        new (Id:entity.Id, Name: entity.Name, Description: entity.Description, CreateDt: entity.CreateDt);

    protected override ICollection<PastTodo_EditOnly> MapToCollectionOfEntity(ICollection<PastTodo_EditOnlyDto> requestModels)
    {
        var data = new List<PastTodo_EditOnly>();
        requestModels.ToList().ForEach(x =>
        {
            data.Add(new PastTodo_EditOnly
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                CreateDt = x.CreateDt,
            });
        });
        return data;
    }

    protected override ICollection<PastTodo_EditOnlyDto> MapToCollectionOfDto(ICollection<PastTodo_EditOnly> entities)
    {
        var data = new List<PastTodo_EditOnlyDto>();
        entities.ToList().ForEach(entity =>
        {
            data.Add(new PastTodo_EditOnlyDto(Id:entity.Id, Name: entity.Name, Description: entity.Description, CreateDt: entity.CreateDt));
        });
        return data;
    }

    public async Task<ICollection<PastTodo_EditOnlyDto>> GetAllAsync()
    {
        var data = await dataHandler.GetAllAsync();
        return MapToCollectionOfDto(data);
    }
}