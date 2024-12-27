## Build By Example

Here is a simple Web API of a **Todo App** using Quickie for **Minimal API** project.

### Step 1: Create a new Web API Project

```bash
dotnet new webapi -n todo.apis
```

### Step 2: Install Quickie

Install Quickie from NuGet:

```bash
dotnet add package Quickie
```

### Step 3: Configure Quickie in `Program.cs`

In your `Program.cs`, configure Quickie as follows:

```csharp
builder.Services.QuickieConfig();

app.AddQuickie();
```

**Explanation:**
- For this project, default configuration will be used as shown above.

More on configuration [here](../configuration.html)

### Step 4: Create a Dto and Entity
```csharp
public record TodoCrudableDto(int Id, string Title, string Description) : CrudDto;
```

```csharp
public class TodoCrudableEntity : CrudEntity
{
    [Key]
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime CreatedDate { get; set; }
}
```

### Step 5: Create your apis

For our Todo app, we need CRUD operations:
- **C** -> Create Todo
- **R** -> Read Todo
- **U** -> Update Todo
- **D** -> Delete Todo

```csharp
app.AddCrudEndpoints<TodoCrudableDto, ITodoService, int>("/api/todos");
```

### Step 6: Request handler (Service layer) Setup

#### Todo Service

```csharp
public interface ITodoService : ICrudRequestHandler<TodoCrudableDto, int>;
```

```csharp
public class TodoService(ITodoRepo dataHandler) : CrudRequestHandler<TodoCrudableDto, TodoCrudableEntity, ITodoRepo, int>(dataHandler), ITodoService
{
    protected override TodoCrudableEntity MapToEntity(TodoCrudableDto request)
    {
        var d =  new TodoCrudableEntity()
            { Id = request.Id, Title = request?.Title, Description = request?.Description, CreatedDate = DateTime.Now };
        return d;
    }

    protected override TodoCrudableDto MapToDto(TodoCrudableEntity request)
    {
        var d = request is not null ?  new TodoCrudableDto(request.Id, request?.Title + " id:" + request?.Id, request?.Description) : default;
        return d;
    }
}
```

> **Note:** Mapping must be done manually. You can use any mapping library or write your own logic.

#### Todo Repository (Data handler)

```csharp
public interface ITodoRepo : ICrudDataHandler<TodoCrudableEntity, int>;
```

```csharp
public class TodoRepo(ApplicationDbContext dbContext) : CrudDataHandler<TodoCrudableEntity, ApplicationDbContext, int>(dbContext), ITodoRepo;
```

### Step 7: Configure Database Context

```csharp
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoCrudableEntity> TodoCrudableEntity { get; set; }
}
```

### Step 8: Register Services in DI

DI registration:

```csharp
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepo, TodoRepo>();
builder.Services.AddScoped<ICrudDataHandler<TodoEntity, int>, TodoRepo>();
```

### Step 9: Making Request

Since **Idempotency** is disabled, `X-Idempotency-Key` header is not required.

#### Example Request:

```bash
### Create Todo
POST http://localhost:5220/api/todos
Content-Type: application/json

{
  "title": "Test Todo yayay",
  "description": "Testing CRUD operations"
}

### Get Todo by ID
GET http://localhost:5220/api/todos/3

### Update Todo
PUT http://localhost:5220/api/todos/1
Content-Type: application/json

{
  "title": "Updated Todo",
  "description": "Updated description"
}

### Delete Todo
DELETE http://localhost:5220/api/todos/1
```
Your Minimal API is now ready with:
* CRUD functionality
* Built-in Idempotency (to prevent duplicate requests which is disabled for default configuration)
* Built-in Rate Limiting (enabled by default)

### Not Just CRUD

Quickie is versatile and supports scenarios beyond CRUD operations:
- **CRUD for Collection:** Bulk create, read, update, and delete operations are supported, making it easy to handle multiple entities in a single request.
- **Readonly:** For entities where only read operations are required.
```csharp
// Get only (Single Entity)
app.AddReadOnlyEndpoints<TodoDto, ISingleTodoReqHandler, string>("/api/get/todos");

// Get only (Collection)
app.AddReadOnlyCollectionEndpoints<TodoDto, IReadTodoReqHandler, DataFilterRequest, string>("/api/getcollection/todos");
```
- **Write-only:** For scenarios where entities can only be written to, but not read.
```csharp
// Write-only API
app.AddWriteOnlyEndpoints<WriteOnlyTodoDto, IWriteOnlyTodoReqHandler>("/api/create/todos");
```
- **Edit-only:** For entities that support updates but not creation or deletion.
```csharp
// Edit-only API
app.AddEditOnlyEndpoints<PastTodo_EditOnlyDto, IEditOnlyTodoReqHandler, string>("/api/editonly/todos");
```
- **Readonly Collections:** You can define collections where only bulk read operations are required, and no modifications are allowed.

You can choose the appropriate functionality based on your application's needs.

That's it! Your fully functional Web API with:
- CRUD functionality
- Built-in **Idempotency** (to prevent duplicate requests)
- Built-in **Rate Limiting** (enabled by default)

### Things to Consider
- **DTOs** should be record types.
- **Entities** are class (reference types).


#### More examples [here](https://github.com/sushantpt/Quickie/tree/master/sample).
