using Entities;
using System.Text.Json;

namespace TaskManager.OverDueTasksWorker;

//C# 12 introduces primary constructors, which provide a concise syntax to declare
//constructors whose parameters are available anywhere in the body of the type.

/// <summary>
/// Service for periodic scanning of the tasks that were overdue to assign them 'OverDue' status
/// </summary>
public class TasksHostedService(
    IHttpClientFactory httpClientFactory,
    ILogger<TasksHostedService> logger) : IHostedService, IDisposable
{
    // execute a method on a thread pool thread at specified intervals
    private Timer? timer = null;

    // the number of executions
    private int executionCount = 0;

    // track whether Dispose has been called.
    private bool disposed = false;

    // time interval to execute a method
    private const int ExecutionIntervalInMinutes = 2;

    //TODO: read from the config
    private const string TaskManagerApiUrl = "https://localhost:7096/tasks/overdue";

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Timed Hosted Service running.");

        timer = new Timer(
            async _ => await FindAndUpdateOverDueTasks(stoppingToken),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(ExecutionIntervalInMinutes)
        );

        return Task.CompletedTask;
    }

    private async Task FindAndUpdateOverDueTasks(CancellationToken ct = default)
    {
        // This method handles an overflow condition by wrapping:
        // if location = Int32.MaxValue, location + 1 = Int32.MinValue.
        // No exception is thrown.
        var count = Interlocked.Increment(ref executionCount);

        logger.LogInformation("Timed Hosted Service is working. Execution time: {Time:G}, Execution count: {Count}",
            DateTime.Now,
            count);

        using HttpClient httpClient = httpClientFactory.CreateClient();
        using HttpRequestMessage httpRequestMessage = new(HttpMethod.Patch, TaskManagerApiUrl);
        using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            IEnumerable<ProjectTask>? updatedTasks = contentStream.Length > 0
                ? await JsonSerializer.DeserializeAsync<IEnumerable<ProjectTask>?>(contentStream)
                : [];

            logger.LogInformation("Updated tasks: {Count}", updatedTasks?.Count());
        }
        else
        {
            logger.LogError("Errors occured: {Errors}, Status code: {Code}",
                await httpResponseMessage.Content.ReadAsStringAsync(),
                httpResponseMessage.StatusCode);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Timed Hosted Service is stopping.");

        timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    // A derived class should not be able to override this method.
    public void Dispose()
    {
        Dispose(disposing: true);
        // you should call GC.SuppressFinalize to prevent
        // finalization code for class objects
        // from executing a second time.
        GC.SuppressFinalize(this);
    }

    // Dispose(bool disposing) executes in two distinct scenarios depending on disposing value.
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called.
        if (!disposed)
        {
            // If disposing equals true, dispose all managed and unmanaged resources.
            // The method has been called directly or indirectly by a user's code.
            if (disposing)
            {
                // Dispose managed resources.
                timer?.Dispose();
            }
            // If disposing equals false the method has been called by the
            // runtime from inside the finalizer and you should not reference
            // other objects.

            // clean up unmanaged resources here.

            disposed = true;
        }
    }
}