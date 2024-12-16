namespace Quickie.DataHandlers.Crud.Implementation;

/// <summary>
/// Abstract class serving as CRUD data handler (Repository layer).
/// </summary>
/// <param name="dbContext">Db context</param>
/// <typeparam name="TEntity">Entity</typeparam>
/// <typeparam name="TDbContext">Db context</typeparam>
/// <typeparam name="TIdType">Type of identifier. eg: int, string, ...</typeparam>
public abstract class CrudDataHandler<TEntity, TDbContext, TIdType>(TDbContext dbContext) : ICrudDataHandler<TEntity, TIdType> where TEntity : CrudEntity where TDbContext : DbContext where TIdType : IComparable
{
    private readonly QuickieOptions _options = GlobalQuickieConfigData.Options;
    
    /// <summary>
    /// Data create request.
    /// </summary>
    /// <param name="request">Data requested to create.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TEntity>> CreateAsync(TEntity request, CancellationToken? cancellationToken = default)
    {
        try
        {
            cancellationToken ??= CancellationToken.None;
            cancellationToken.Value.ThrowIfCancellationRequested();
            await dbContext.Set<TEntity>().AddAsync(request, (CancellationToken)cancellationToken);
            var created = await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);
            return new ResponseObj<TEntity>
            { 
                IsSuccess = created > 0,
                Message = created > 0 ? "Created." : "Data not created.",
                Data = request
            };
        }
        catch (Exception ex)
        {
            var errorMsg = _options.ShowCustomErrorMessage ? "Request to create was not completed." : ex.Message;
            return new ResponseObj<TEntity>
            { 
                IsSuccess = false,
                Message = errorMsg,
                Data = request
            };
        }
    }

    /// <summary>
    /// Get data by providing an identifier
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TEntity>> GetByIdAsync(TIdType id, CancellationToken? cancellationToken = default)
    {
        try
        {
            cancellationToken ??= CancellationToken.None;
            cancellationToken.Value.ThrowIfCancellationRequested();
            var data = await dbContext.Set<TEntity>().FindAsync(id, cancellationToken);
            return new ResponseObj<TEntity>
            {
                IsSuccess = data != null,
                Message = data != null ? "Data found." : "Data not found.",
                Data = data!
            };
        }
        catch (Exception ex)
        {
            var errorMsg = _options.ShowCustomErrorMessage ? "Request to GET data was not completed." : ex.Message;
            return new ResponseObj<TEntity>
            { 
                IsSuccess = false,
                Message = errorMsg,
                Data = null!
            };
        }
    }

    /// <summary>
    /// Update request.
    /// </summary>
    /// <param name="id">Identifier</param>
    /// <param name="request">Requested data identifier.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj<TEntity>> UpdateAsync(TIdType id, TEntity request, CancellationToken? cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nameof(id));
            ArgumentNullException.ThrowIfNull(request);
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
            dbContext.Entry(request).State = EntityState.Modified;
            // set the id property of the new entity to match the route parameter
            var idProperty = dbContext.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.Single();
            idProperty?.PropertyInfo?.SetValue(request, id);

            try
            {
                var update = await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);

                return new ResponseObj<TEntity>
                {
                    IsSuccess = update > 0,
                    Message = update > 0 ? "Entity updated successfully" : "No changes were made. Update failed",
                    Data = request
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
    /// Delete request.
    /// </summary>
    /// <param name="id">Identifier of the data to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task<ResponseObj> DeleteAsync(TIdType id, CancellationToken? cancellationToken = default)
    {
        try
        {
            cancellationToken ??= CancellationToken.None;
            cancellationToken.Value.ThrowIfCancellationRequested();
            
            var dataExists = await dbContext.Set<TEntity>().FindAsync(id, cancellationToken);
            if (dataExists is null)
                return new ResponseObj{IsSuccess = false, Message = "Data not found." };
            dbContext.Set<TEntity>().Remove(dataExists);
            var deleteCount = await dbContext.SaveChangesAsync((CancellationToken)cancellationToken);
            return 
                deleteCount.Equals(1) 
                    ? new ResponseObj{IsSuccess = true, Message = "Data removed successfully."} 
                    : new ResponseObj{IsSuccess = false, Message = "Something went wrong."};
        }
        catch (Exception ex)
        {
            var errorMsg = _options.ShowCustomErrorMessage ? "Request to delete data was not completed." : ex.Message;
            return new ResponseObj{ IsSuccess = false, Message = errorMsg };
        }
    }
}