using TaskManager.OverDueTasksWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddHttpClient(); // Register IHttpClientFactory

builder.Services.AddHostedService<TasksHostedService>(); // singleton

var host = builder.Build();
host.Run();
