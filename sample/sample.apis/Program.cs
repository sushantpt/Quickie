using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quickie.Configuration;
using Quickie.Configuration.Idempotency;
using Quickie.Configuration.Options;
using sample.apis.Data;
using sample.apis.Repositories;
using sample.apis.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sample APIS",
        Version = "v1",
        Description = "API Documentation for Sample App"
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("SampleAppDb")); 

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepo, TodoRepo>();

builder.Services.QuickieConfig(options => {
    options.ShowCustomErrorMessage = false;
    options.RateLimitingConfiguration = new RateLimitConfiguration
    {
        DisableRateLimiting = false
    };
    options.IdempotencyConfiguration = new IdempotentConfiguration
    {
        Enable = true
    };
});

var app = builder.Build();

app.AddQuickie();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
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
