using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Readonly.Implementation;
using specification.Helpers.Entities;

namespace specification.Helpers.DataHandlers;

public class ReadOnlyDataHandlerConcrete(DbContext context) : ReadOnlyDataHandler<ReadOnlyEntityConcrete,DbContext,int>(context);