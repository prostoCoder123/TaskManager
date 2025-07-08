var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("db").WithPgAdmin(); //adds a container based on the docker.io/dpage/pgadmin4 image
var tasksDb = db.AddDatabase("tasksdb");

var migrations = builder.AddProject<Projects.TaskManager_MigrationService>("migrations")
    .WithReference(tasksDb)
    .WaitFor(tasksDb);

var taskManager = builder.AddProject<Projects.TaskManager>("taskmanager")
    .WithReference(tasksDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);

// Add the Vue.js project as an NpmApp
builder.AddNpmApp("vue", "../taskmanager.ui")
    .WithReference(taskManager)
    .WaitFor(taskManager)
    .WithHttpEndpoint(port: 65089, targetPort: 65088) //TODO
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var taskWorker = taskManager = builder.AddProject<Projects.TaskManager_OverDueTasksWorker>("taskworker")
    .WaitFor(taskManager);

builder.Build().Run();