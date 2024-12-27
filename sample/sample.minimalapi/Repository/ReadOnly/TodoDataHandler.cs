using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Readonly.Implementation;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.ReadOnly;

public class TodoDataHandler(DbContext context) : ReadOnlyCollectionDataHandler<TodoEntity, ApplicationDbContext, string>(context), ITodoDataHandler;