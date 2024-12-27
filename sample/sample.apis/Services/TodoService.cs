using Quickie.Handlers.Crud.Implementation;
using sample.apis.Dtos;
using sample.apis.Entities;
using sample.apis.Repositories;

namespace sample.apis.Services;

public class TodoService(ITodoRepo dataHandler) : CrudRequestHandler<TodoDto, TodoEntity, ITodoRepo, int>(dataHandler), ITodoService
{
    protected override TodoEntity MapToEntity(TodoDto request)
    {
        var d =  new TodoEntity()
            { Id = request.Id, Title = request?.Title, Description = request?.Description, CreatedDate = DateTime.Now };
        return d;
    }

    protected override TodoDto MapToDto(TodoEntity request)
    {
        var d = request is not null ?  new TodoDto(request.Id, request?.Title + " id:" + request?.Id, request?.Description) : default;
        return d;
    }
}