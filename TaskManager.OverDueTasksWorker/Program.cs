using TaskManager.OverDueTasksWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Register IHttpClientFactory
// (retry policy is configured in ConfigureHttpClientDefaults -> AddStandardResilienceHandler)
builder.Services.AddHttpClient();

builder.Services.AddHostedService<TasksHostedService>(); // singleton

var host = builder.Build();
host.Run();