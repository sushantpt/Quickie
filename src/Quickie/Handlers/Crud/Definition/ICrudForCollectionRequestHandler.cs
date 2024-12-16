namespace Quickie.Handlers.Crud.Definition;

public interface ICrudForCollectionRequestHandler<TViewModel, TIdType> where TViewModel : CrudDto where TIdType : IComparable
{
    Task<ResponseObj<ICollection<TViewModel>>> CreateRangeAsync(ICollection<TViewModel> entities, CancellationToken? cancellationToken = default);
    
    Task<ResponseObj<ICollection<TViewModel>>> UpdateRangeAsync(ICollection<TViewModel> entities, CancellationToken? cancellationToken = default);

    Task<ResponseObj> DeleteRangeAsync(ICollection<TIdType> ids, CancellationToken? cancellationToken = default);

    Task<ResponseObj<PaginatedDataObj<TViewModel>>> GetAllAsync(RequestForDataObj request, CancellationToken? cancellationToken = default);

    Task<ResponseObj<int>> CountAsync(CancellationToken? cancellationToken = default);
}