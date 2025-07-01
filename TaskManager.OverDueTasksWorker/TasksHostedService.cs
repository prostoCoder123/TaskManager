using Entities;
using System.Text.Json;

namespace TaskManager.OverDueTasksWorker;

public class TasksHostedService(
    IHttpClientFactory httpClientFactory,
    ILogger<TasksHostedService> logger) : IHostedService, IDisposable
{
    private Timer? timer = null;
    private int executionCount = 0;
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

    public void Dispose()
    {
        timer?.Dispose();
    }
}
