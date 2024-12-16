namespace Quickie.DataHandlers.Writeonly.Definition;

public interface IWriteOnlyDataHandler<TEntity> where TEntity : WriteOnlyEntity
{
    Task<ResponseObj<TEntity>> CreateItemAsync(TEntity item, CancellationToken? cancellationToken); 
    Task<ResponseObj<ICollection<TEntity>>> CreateItemsAsync(ICollection<TEntity> items, CancellationToken? cancellationToken); 
}