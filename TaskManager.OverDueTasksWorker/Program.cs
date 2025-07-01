using TaskManager.OverDueTasksWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddHttpClient(); // Register IHttpClientFactory
//TODO: Polly retry policy

builder.Services.AddHostedService<TasksHostedService>(); // singleton

var host = builder.Build();
host.Run();
