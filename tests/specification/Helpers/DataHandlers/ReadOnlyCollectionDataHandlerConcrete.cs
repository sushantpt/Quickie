using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Readonly.Implementation;
using specification.Helpers.Entities;

namespace specification.Helpers.DataHandlers;

public class ReadOnlyCollectionDataHandlerConcrete(DbContext context) : ReadOnlyCollectionDataHandler<ReadOnlyEntityConcrete,DbContext,int>(context)
{
    
}