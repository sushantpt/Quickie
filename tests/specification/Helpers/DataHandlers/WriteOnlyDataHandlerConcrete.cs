using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Writeonly.Implementation;
using specification.Helpers.Entities;

namespace specification.Helpers.DataHandlers;

public class WriteOnlyDataHandlerConcrete(DbContext context) : WriteOnlyDataHandler<WriteOnlyEntityConcrete, DbContext>(context);