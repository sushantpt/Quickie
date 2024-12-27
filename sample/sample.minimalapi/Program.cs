using Microsoft.EntityFrameworkCore;
using Quickie.Configuration;
using Quickie.MinimalApis.Crud;
using Quickie.MinimalApis.EditOnly;
using Quickie.MinimalApis.ReadOnly;
using Quickie.MinimalApis.WriteOnly;
using sample.minimalapi;
using sample.minimalapi.Dtos;
using sample.minimalapi.Repository.Crud;
using sample.minimalapi.Repository.EditOnly;
using sample.minimalapi.Repository.ReadOnly;
using sample.minimalapi.Repository.WriteOnly;
using sample.minimalapi.Services.Crud;
using sample.minimalapi.Services.EditOnly;
using sample.minimalapi.Services.ReadOnly;
using sample.minimalapi.Services.WriteOnly;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SampleReadOnlyAppDb")); 

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepo, TodoRepo>();
builder.Services.AddScoped<DbContext, ApplicationDbContext>();
builder.Services.AddScoped<IReadTodoReqHandler, ReadIReadTodoReqHandler>();
builder.Services.AddScoped<ITodoDataHandler, TodoDataHandler>();
builder.Services.AddScoped<ISingleTodoReqHandler, SingleTodoReqHandler>();
builder.Services.AddScoped<ISingleTodoDataHandler, SingleTodoDataHandler>();
builder.Services.AddScoped<IEditOnlyTodoReqHandler, EditOnlyTodoReqHandler>();
builder.Services.AddScoped<IEditOnlyTodoDataHandler, EditOnlyTodoDataHandler>();
builder.Services.AddScoped<IWriteOnlyTodoReqHandler, WriteOnlyTodoReqHandler>();
builder.Services.AddScoped<IWriteOnlyTodoDataHandler, WriteOnlyTodoDataHandler>();
builder.Services.AddScoped<DataSeed>();

builder.Services.QuickieConfig();
builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dataSeeder = services.GetRequiredService<DataSeed>();
    await dataSeeder.GenerateTodoDataAsync();
}
app.AddQuickie();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// crud apis
app.AddCrudEndpoints<TodoCrudableDto, ITodoService, int>("/api/todos");

// write only api
app.AddWriteOnlyEndpoints<WriteOnlyTodoDto, IWriteOnlyTodoReqHandler>("/api/create/todos");

// get only
app.AddReadOnlyEndpoints<TodoDto, ISingleTodoReqHandler, string>("/api/get/todos");
app.AddReadOnlyCollectionEndpoints<TodoDto, IReadTodoReqHandler, DataFilterRequest, string>("/api/getcollection/todos");

// edit only
app.AddEditOnlyEndpoints<PastTodo_EditOnlyDto, IEditOnlyTodoReqHandler, string>("/api/editonly/todos");

await app.RunAsync();


