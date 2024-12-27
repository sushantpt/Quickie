using Quickie.Handlers.Crud.Implementation;
using sample.minimalapi.Dtos;
using sample.minimalapi.Entities;
using sample.minimalapi.Repository.Crud;

namespace sample.minimalapi.Services.Crud;

public class TodoService(ITodoRepo dataHandler) : CrudRequestHandler<TodoCrudableDto, TodoCrudableEntity, ITodoRepo, int>(dataHandler), ITodoService
{
    protected override TodoCrudableEntity MapToEntity(TodoCrudableDto request)
    {
        var d =  new TodoCrudableEntity()
            { Id = request.Id, Title = request?.Title, Description = request?.Description, CreatedDate = DateTime.Now };
        return d;
    }

    protected override TodoCrudableDto MapToDto(TodoCrudableEntity request)
    {
        var d = request is not null ?  new TodoCrudableDto(request.Id, request?.Title + " id:" + request?.Id, request?.Description) : default;
        return d;
    }
}