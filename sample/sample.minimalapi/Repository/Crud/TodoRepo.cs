using Quickie.DataHandlers.Crud.Implementation;
using sample.minimalapi.Entities;

namespace sample.minimalapi.Repository.Crud;

public class TodoRepo(ApplicationDbContext dbContext) : CrudDataHandler<TodoCrudableEntity, ApplicationDbContext, int>(dbContext), ITodoRepo;