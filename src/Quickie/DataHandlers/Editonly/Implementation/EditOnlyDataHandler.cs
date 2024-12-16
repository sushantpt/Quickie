namespace Quickie.DataHandlers.Editonly.Implementation;

/// <summary>
/// Abstract class serving as edit only data handler (Repository layer).
/// </summary>
/// <param name="dbContext">Db context</param>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class EditOnlyDataHandler<TEntity, TIdType>(DbContext dbContext) : IEditOnlyDataHandler<TEntity, TIdType> where TEntity : EditOnlyEntity where TIdType : IComparable
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    /// <summary>
    /// Update request.
    /// </summary>
    /// <param name="id">Identifier of the entity</param>
    /// <param name="requestModel">Requested data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TEntity>> EditAsync(TIdType id, TEntity requestModel, CancellationToken? cancellationToken = default)
    { 
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(id));
            ArgumentNullException.ThrowIfNull(requestModel);
            if (typeof(TIdType) == typeof(int) && id is < 1)
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be negative or zero.", nameof(id));
            }
            cancellationToken ??= CancellationToken.None;
            cancellationToken.Value.ThrowIfCancellationRequested();
            
            var existingEntity = await dbContext.Set<TEntity>().FindAsync( id, cancellationToken);
            if (existingEntity == null)
            {
                return new ResponseObj<TEntity>
                {
                    IsSuccess = false,
                    Message = $"Data with ID {id} not found",
                    Data = null!
                };
            }

            // detach the existing entity to avoid tracking conflicts
            dbContext.Entry(existingEntity).State = EntityState.Detached;
            // attach the new entity and mark it as modified
            dbContext.Entry(requestModel).State = EntityState.Modified;
            // set the id property of the new entity to match the route parameter
            var idProperty = dbContext.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Single();
            idProperty?.PropertyInfo?.SetValue(requestModel, id);

            try
            {
                var update = await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);

                return new ResponseObj<TEntity>
                {
                    IsSuccess = update > 0,
                    Message = update > 0 ? "Entity updated successfully" : "No changes were made. Update failed",
                    Data = requestModel
                };
            }
            catch (DbUpdateConcurrencyException de)
            {
                return new ResponseObj<TEntity>
                {
                    IsSuccess = false,
                    Message = _options.ShowCustomErrorMessage ? "The entity was modified by another user!" : de.Message,
                    Data = null!
                };
            }
        }
        catch (Exception ex)
        {
            return new ResponseObj<TEntity>
            {
                IsSuccess = false,
                Message = _options.ShowCustomErrorMessage ? "An error occurred while updating the data." : ex.Message,
                Data = null!
            };
        }
    }

    /// <summary>
    /// Update collection of data.
    /// </summary>
    /// <param name="requestModels">Collection of requested data to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<ICollection<TEntity>>> EditCollectionAsync(ICollection<TEntity> requestModels, CancellationToken? cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(requestModels));
        var response = new ResponseObj<ICollection<TEntity>>();
        cancellationToken ??= CancellationToken.None;
        cancellationToken.Value.ThrowIfCancellationRequested();
        try
        {
            await using var transaction = (await dbContext.Database.BeginTransactionAsync());
            dbContext.Set<TEntity>().UpdateRange(requestModels);
            var updatedEntities = await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);
            await transaction.CommitAsync((CancellationToken)cancellationToken);
            response.IsSuccess = updatedEntities > 0;
            response.Data = requestModels;
            response.Message = updatedEntities > 0 ? "Updated." : "No changes were made. Update failed.";
            return response;
        }
        catch (Exception e)
        {
            response.IsSuccess = false;
            response.Message = _options.ShowCustomErrorMessage ? "An error occurred while updating the data." : e.Message;
            response.Data = requestModels;
            return response;
        }
    }
}