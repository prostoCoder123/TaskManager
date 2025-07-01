using Entities;
using System.Text.Json;

namespace TaskManager.OverDueTasksWorker;

public class TasksHostedService(
    IHttpClientFactory httpClientFactory,
    ILogger<TasksHostedService> logger) : IHostedService, IDisposable
{
    private Timer? _timer = null;
    private int executionCount = 0;
    private const int executionIntervalMinutes = 2;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(
            async _ => await FindAndUpdateOverDueTasks(stoppingToken),
            null,
            TimeSpan.Zero,
            TimeSpan.FromMinutes(executionIntervalMinutes)
        );

        return Task.CompletedTask;
    }

    private async Task FindAndUpdateOverDueTasks(CancellationToken ct = default)
    {
        var count = Interlocked.Increment(ref executionCount);

        logger.LogInformation("Timed Hosted Service is working. Execution time: {Time:G}, Execution count: {Count}",
            DateTime.Now,
            count);

        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Patch,
            "https://localhost:7096/tasks/overdue"); //TODO

        var httpClient = httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

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
            // TODO: test
            logger.LogError("Errors occured: {Errors}", await httpResponseMessage.Content.ReadAsStreamAsync());
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
