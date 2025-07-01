using TaskManager.OverDueTasksWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Register IHttpClientFactory
builder.Services.AddHttpClient(); // TODO: configure Polly retry policy

builder.Services.AddHostedService<TasksHostedService>(); // singleton

var host = builder.Build();
host.Run();
