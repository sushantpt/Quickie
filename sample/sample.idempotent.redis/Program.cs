using Microsoft.EntityFrameworkCore;
using Quickie.Configuration;
using Quickie.Configuration.Idempotency;
using Quickie.MinimalApis.Crud;
using sample.idempotent.redis;
using sample.idempotent.redis.Configuration;
using sample.idempotent.redis.Dtos;
using sample.idempotent.redis.Repository.Crud;
using sample.idempotent.redis.Services.Crud;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SampleReadOnlyAppDb"));

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepo, TodoRepo>();

builder.Services.AddOpenApi();
var redisConnection = await ConnectionMultiplexer.ConnectAsync("localhost:6379");

builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

builder.Services.QuickieConfig(options => {
    options.IdempotencyConfiguration = new IdempotentConfiguration {
        Enable = true,
        RunBackgroundServiceEveryHour = 1,
        IdempotencyLifespan = TimeSpan.FromHours(2),
        Provider = new RedisIdempotencyProvider(redisConnection)
    };
});

var app = builder.Build();

app.AddQuickie();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// crud api
app.AddCrudEndpoints<TodoCrudableDto, ITodoService, int>("/api/todos");


await app.RunAsync();
