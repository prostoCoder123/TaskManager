using Entities;
using TaskManager;
using TaskManager.Abstractions;
using TaskManager.EfCore;
using TaskManager.Implementations;
using TaskManager.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProjectTaskContext>("tasksdb");

builder.Services.AddResponseCaching();

builder.Services.AddScoped<ErrorHandlingFilterService>();

// Add services to the container.
builder.Services
    .AddAutoMapper(c => c.AddProfile<TaskMappingProfile>())
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<IGenericRepository<ProjectTask>, TaskRepository>()
    .AddScoped<IUnitOfWork, UnitOfWork>()
    .AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapDefaultEndpoints(); // healthchecks
}

app.UseResponseCaching();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();