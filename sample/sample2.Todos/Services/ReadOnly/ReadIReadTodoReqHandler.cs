using System.Linq.Expressions;
using Quickie.DataHandlers.Readonly.Definition;
using Quickie.DataObj;
using Quickie.Handlers.Readonly.Implementation;
using sample2.Todos.Dtos;
using sample2.Todos.Entities;
using sample2.Todos.Repository;
using sample2.Todos.Repository.ReadOnly;

namespace sample2.Todos.Services.ReadOnly;

public class ReadIReadTodoReqHandler(IReadOnlyCollectionDataHandler<TodoEntity, string> readOnlyCollectionDataHandler) : ReadOnlyCollectionRequestHandler<TodoDto, TodoEntity, TodoDataHandler ,string>(readOnlyCollectionDataHandler), IReadTodoReqHandler
{
    private readonly IReadOnlyCollectionDataHandler<TodoEntity, string> _readOnlyCollectionDataHandler = readOnlyCollectionDataHandler;

    public new async Task<PaginatedDataObj<TodoDto>> GetAsync<TRequestModel>(TRequestModel? request, CancellationToken? cancellationToken) where TRequestModel : RequestForDataObj
    {
        var requestFilter = request as DataFilterRequest;
        Expression<Func<TodoEntity, bool>> predicate = x => requestFilter != null && x.Name.Contains(requestFilter.Name);
        
        var data = await _readOnlyCollectionDataHandler.GetByFilterAsync(predicate, requestFilter, false, cancellationToken);
        return new PaginatedDataObj<TodoDto>()
        {
            Items = MapToDto(data.Items.ToList()),
            Total = data!.Total,
        };
    }

    protected override ICollection<TodoDto> MapToDto(ICollection<TodoEntity> entity)
    {
        var data = new List<TodoDto>();
        entity.ToList().ForEach(x => data.Add(new TodoDto(Id: x.Id, Name: x.Name, Description: x.Description, CreateDt: x.CreateDt)));
        return data;
    }
}