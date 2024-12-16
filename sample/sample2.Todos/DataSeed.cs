using Bogus;
using sample2.Todos.Entities;

namespace sample2.Todos;

public class DataSeed(ApplicationDbContext context)
{
    public async Task GenerateTodoDataAsync()
    {
        var data = new Faker<TodoEntity>()
            .RuleFor(t => t.Id, f => Guid.NewGuid().ToString())
            .RuleFor(t => t.Name, f => f.Name.FullName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.CreateDt, f => f.Date.Past());
        
        var data1 = new Faker<PastTodo_EditOnly>()
            .RuleFor(t => t.Id, f => Guid.NewGuid().ToString())
            .RuleFor(t => t.Name, f => f.Name.FullName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.CreateDt, f => f.Date.Past());

        var todos = data.Generate(200);
        var pastTodos = data1.Generate(200);
        
        await context.Set<TodoEntity>().AddRangeAsync(todos);
        await context.Set<PastTodo_EditOnly>().AddRangeAsync(pastTodos);
        
        await context.SaveChangesAsync();
        await Task.CompletedTask;
    }
}