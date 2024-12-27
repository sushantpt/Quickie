## Build By Example

Here is a simple Web API with controllers for a **Todo App** using Quickie.

### Step 1: Create a new Web API Project

```bash
dotnet new webapi -n todo.apis --use-controllers
```

### Step 2: Install Quickie

Install Quickie from NuGet:

```bash
dotnet add package Quickie
```

### Step 3: Configure Quickie in `Program.cs`

In your `Program.cs`, configure Quickie as follows:

```csharp
builder.Services.QuickieConfig(options => {
    options.ShowCustomErrorMessage = true;
    options.RateLimitingConfiguration = new RateLimitConfiguration
    {
        DisableRateLimiting = false
    };
    options.IdempotencyConfiguration = new IdempotentConfiguration
    {
        Enable = true
    };
});

app.AddQuickie();
```

**Explanation:**
- For this project, we are enabling **Idempotency**.
- Rate limiting is **enabled by default**, but we chose to explicitly configure it here.
- Custom error messages are shown when exceptions occur.

### Step 4: Create a Dto and Entity
```csharp
public record TodoDto(int Id, string Title, string Description) : CrudDto;
```

```csharp
public class TodoEntity : CrudEntity
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedDate { get; set; }
}
```

### Step 5: Create a Controller

For our Todo app, we need CRUD operations:
- **C** -> Create Todo
- **R** -> Read Todo
- **U** -> Update Todo
- **D** -> Delete Todo

Here is the `TodoController`:

```csharp
public class TodoController(ITodoService requestHandler) : CrudController<TodoDto, ITodoService, int>(requestHandler);
```

### Step 6: Request handler (Service layer) Setup

#### Todo Service

```csharp
public interface ITodoService : ICrudRequestHandler<TodoDto, int>;

public class TodoService(ICrudDataHandler<TodoEntity, int> dataHandler) : CrudRequestHandler<TodoDto, TodoEntity, ITodoRepo, int>(dataHandler), ITodoService
{
    protected override TodoEntity MapToEntity(TodoDto request)
    {
        var d = new TodoEntity()
        {
            Id = request.Id,
            Title = request?.Title,
            Description = request?.Description,
            CreatedDate = DateTime.Now
        };
        return d;
    }

    protected override TodoDto MapToDto(TodoEntity request)
    {
        var d = request is not null ?  new TodoDto(request.Id, request?.Title + " id:" + request?.Id, request?.Description) : default;
        return d;
    }
}
```

> **Note:** Mapping must be done manually. You can use any mapping library or write your own logic.

#### Todo Repository (Data handler)

```csharp
public interface ITodoRepo : ICrudDataHandler<TodoEntity, int>;

public class TodoRepo(ApplicationDbContext dbContext) : CrudDataHandler<TodoEntity, ApplicationDbContext, int>(dbContext), ITodoRepo;
```

### Step 7: Configure Database Context

```csharp
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoEntity> TodoEntity { get; set; }
}
```

### Step 8: Register Services in DI

Register the services in `Program.cs`:

```csharp
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepo, TodoRepo>();
builder.Services.AddScoped<ICrudDataHandler<TodoEntity, int>, TodoRepo>();
```

### Step 9: Making Requests with Idempotency

Since **Idempotency** is enabled, you must provide an `X-Idempotency-Key` with each request (POST calls). For duplicate requests, the API will respond with a **409 Conflict** status.

#### Example Request:

```bash
curl -X 'POST' \
  'http://localhost:5162/api/Todo' \
  -H 'accept: application/json' \
  -H 'X-Idempotency-Key: c311bef0-9953-45b1-bb73-70169e1a3de5' \
  -H 'Content-Type: application/json' \
  -d '{
  "id": 0,
  "title": "work",
  "description": "feature 0"
}'
```

### Not Just CRUD

Quickie is versatile and supports scenarios beyond CRUD operations:
- **CRUD for Collection:** Bulk create, read, update, and delete operations are supported, making it easy to handle multiple entities in a single request.
- **Readonly:** For entities where only read operations are required.
- **Write-only:** For scenarios where entities can only be written to, but not read.
- **Edit-only:** For entities that support updates but not creation or deletion.
- **Readonly Collections:** You can define collections where only bulk read operations are required, and no modifications are allowed.

You can choose the appropriate functionality based on your application's needs.

### API is Ready!

That's it! Your fully functional Web API with:
- CRUD functionality
- Built-in **Idempotency** (to prevent duplicate requests)
- Built-in **Rate Limiting** (enabled by default)

### Things to Consider
- **DTOs** should be record types.
- **Entities** are class (reference types).


#### More examples [here](https://github.com/sushantpt/Quickie/tree/master/sample).
