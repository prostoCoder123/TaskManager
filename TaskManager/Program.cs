using Entities;
using TaskManager.Abstractions;
using TaskManager.EfCore;
using TaskManager.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProjectTaskContext>("tasksdb");

// Add services to the container.
builder.Services
    .AddTransient<ITaskRepository, TaskRepository>()
    .AddTransient<IGenericRepository<ProjectTask>, TaskRepository>()
    .AddTransient<IUnitOfWork, UnitOfWork>()
    .AddTransient<ITaskService, TaskService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
