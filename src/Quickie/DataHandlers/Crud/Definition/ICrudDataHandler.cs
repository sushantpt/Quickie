namespace Quickie.DataHandlers.Crud.Definition;

public interface ICrudDataHandler<TEntity, in TIdType> where TEntity : CrudEntity where TIdType : IComparable
{
    Task<ResponseObj<TEntity>> CreateAsync(TEntity request, CancellationToken? cancellationToken);
    Task<ResponseObj<TEntity>> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default);
    Task<ResponseObj<TEntity>> UpdateAsync(TIdType id, TEntity request, CancellationToken? cancellationToken = default);
    Task<ResponseObj> DeleteAsync(TIdType id, CancellationToken? cancellationToken = default);
}