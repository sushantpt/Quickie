using Microsoft.EntityFrameworkCore;
using Quickie.DataHandlers.Readonly.Implementation;
using sample2.Todos.Entities;

namespace sample2.Todos.Repository.ReadOnly;

public class SingleTodoDataHandler(DbContext context) : ReadOnlyDataHandler<TodoEntity, ApplicationDbContext, string>(context), ISingleTodoDataHandler;