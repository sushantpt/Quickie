namespace Quickie.Handlers.Writeonly.Definition;

public interface IWriteOnlyRequestHandler<TViewModel> where TViewModel : WriteOnlyDto
{
    Task<ResponseObj<TViewModel>> CreateItemAsync(TViewModel item, CancellationToken? cancellationToken); 
    Task<ResponseObj<ICollection<TViewModel>>> CreateItemsAsync(ICollection<TViewModel> items, CancellationToken? cancellationToken); 
}