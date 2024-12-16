namespace Quickie.DataHandlers.Crud.Definition;

public interface ICrudForCollectionDataHandler<TEntity, TIdType> where TEntity : CrudEntity where TIdType : IComparable
{
    Task<ResponseObj<ICollection<TEntity>>> CreateRangeAsync(ICollection<TEntity> entities, CancellationToken? cancellationToken = default);
    
    Task<ResponseObj<ICollection<TEntity>>> UpdateRangeAsync(ICollection<TEntity> entities, CancellationToken? cancellationToken = default);

    Task<ResponseObj> DeleteRangeAsync(ICollection<TIdType> ids, CancellationToken? cancellationToken = default);

    Task<ResponseObj<PaginatedDataObj<TEntity>>> GetAllAsync(RequestForDataObj request, CancellationToken? cancellationToken = default);

    Task<ResponseObj<int>> CountAsync(CancellationToken? cancellationToken = default);
}