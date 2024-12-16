using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Editonly.Implementation;
using specification.Helpers.Entities;

namespace specification.Helpers.DataHandlers;

public class EditOnlyDataHandlerConcrete(DbContext dbContext) : EditOnlyDataHandler<EditOnlyEntityConcrete, int>(dbContext)
{
    
}