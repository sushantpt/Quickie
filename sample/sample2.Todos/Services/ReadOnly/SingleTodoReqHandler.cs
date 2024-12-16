using Quickie.Handlers.Readonly.Implementation;
using sample2.Todos.Dtos;
using sample2.Todos.Entities;
using sample2.Todos.Repository;
using sample2.Todos.Repository.ReadOnly;

namespace sample2.Todos.Services.ReadOnly;

public class SingleTodoReqHandler(ISingleTodoDataHandler dataHandler) : ReadOnlyRequestHandler<TodoDto, TodoEntity, ISingleTodoDataHandler, string>(dataHandler), ISingleTodoReqHandler
{
    protected override TodoDto MapToDto(TodoEntity entity) =>
        new (Id: entity.Id, Name: entity.Name, Description: entity.Description, CreateDt: entity.CreateDt);
}