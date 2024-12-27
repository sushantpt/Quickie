using Quickie.DataHandlers.Crud.Implementation;
using sample.idempotent.redis.Entities;

namespace sample.idempotent.redis.Repository.Crud;

public class TodoRepo(ApplicationDbContext dbContext) : CrudDataHandler<TodoCrudableEntity, ApplicationDbContext, int>(dbContext), ITodoRepo;