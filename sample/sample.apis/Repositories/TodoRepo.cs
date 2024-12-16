using Quickie.DataHandlers.Crud.Implementation;
using sample.apis.Data;
using sample.apis.Entities;

namespace sample.apis.Repositories;

public class TodoRepo(ApplicationDbContext dbContext) : CrudDataHandler<TodoEntity, ApplicationDbContext, int>(dbContext), ITodoRepo;