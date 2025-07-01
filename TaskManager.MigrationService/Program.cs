
using TaskManager.EfCore;
using TaskManager.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<MigrationWorker>();

builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(MigrationWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<ProjectTaskContext>("tasksdb");

var host = builder.Build();

host.Run();