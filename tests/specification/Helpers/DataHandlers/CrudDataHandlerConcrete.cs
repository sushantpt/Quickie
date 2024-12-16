using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Crud.Implementation;
using specification.Helpers.Entities;

namespace specification.Helpers.DataHandlers;

public class CrudDataHandlerConcrete(DbContext dbContext) : CrudDataHandler<CrudEntityConcrete,DbContext,int>(dbContext)
{
    
}