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

var taskWorker = taskManager = builder.AddProject<Projects.TaskManager_OverDueTasksWorker>("taskworker")
    .WaitFor(taskManager);

builder.Build().Run();