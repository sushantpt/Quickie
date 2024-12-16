using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quickie.Configuration;
using sample2.Todos;
using sample2.Todos.Repository;
using sample2.Todos.Repository.EditOnly;
using sample2.Todos.Repository.ReadOnly;
using sample2.Todos.Repository.WriteOnly;
using sample2.Todos.Services;
using sample2.Todos.Services.EditOnly;
using sample2.Todos.Services.ReadOnly;
using sample2.Todos.Services.WriteOnly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo (Readonly) APIS",
        Version = "v1",
        Description = "API Documentation for Todo App"
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SampleReadOnlyAppDb")); 

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dataSeeder = services.GetRequiredService<DataSeed>();
    await dataSeeder.GenerateTodoDataAsync();
}

app.AddQuickie();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API V1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();