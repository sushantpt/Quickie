using Quickie.Handlers.Readonly.Implementation;
using sample.minimalapi.Dtos;
using sample.minimalapi.Entities;
using sample.minimalapi.Repository.ReadOnly;

namespace sample.minimalapi.Services.ReadOnly;

public class SingleTodoReqHandler(ISingleTodoDataHandler dataHandler) : ReadOnlyRequestHandler<TodoDto, TodoEntity, ISingleTodoDataHandler, string>(dataHandler), ISingleTodoReqHandler
{
    protected override TodoDto MapToDto(TodoEntity entity) =>
        new (Id: entity.Id, Name: entity.Name, Description: entity.Description, CreateDt: entity.CreateDt);
}