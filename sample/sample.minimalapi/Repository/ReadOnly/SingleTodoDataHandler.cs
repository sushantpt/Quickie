using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Readonly.Implementation;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.ReadOnly;

public class SingleTodoDataHandler(DbContext context) : ReadOnlyDataHandler<TodoEntity, ApplicationDbContext, string>(context), ISingleTodoDataHandler;