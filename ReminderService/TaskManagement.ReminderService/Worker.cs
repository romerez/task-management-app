using TaskManagement.ReminderService.Models;
using TaskManagement.ReminderService.Services;

namespace TaskManagement.ReminderService;

public class ReminderWorker : BackgroundService
{
    private readonly ILogger<ReminderWorker> _logger;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ITaskReminderService _taskReminderService;
    private readonly IConfiguration _configuration;

    private TimeSpan CheckInterval => TimeSpan.FromMinutes(
        _configuration.GetValue<int>("ReminderService:CheckIntervalMinutes", 5));

    public ReminderWorker(
        ILogger<ReminderWorker> logger,
        IRabbitMQService rabbitMQService,
        ITaskReminderService taskReminderService,
        IConfiguration configuration)
    {
        _logger = logger;
        _rabbitMQService = rabbitMQService;
        _taskReminderService = taskReminderService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("===========================================");
        _logger.LogInformation("Task Reminder Service starting...");
        _logger.LogInformation("Check interval: {Interval}", CheckInterval);
        _logger.LogInformation("===========================================");

        StartMessageConsumer();
        await RunCheckLoopAsync(stoppingToken);
    }

    private void StartMessageConsumer()
    {
        _logger.LogInformation("Starting message consumer...");
        _rabbitMQService.StartConsuming(HandleReminderMessage);
        _logger.LogInformation("Message consumer started successfully");
    }

    private void HandleReminderMessage(TaskReminderMessage message)
    {
        _logger.LogWarning(
            "Hi your Task is due {{Task {TaskTitle}}} - Due: {DueDate:yyyy-MM-dd}, Overdue by {DaysOverdue} days, Owner: {Owner} ({Email})",
            message.TaskTitle,
            message.DueDate,
            message.DaysOverdue,
            message.UserFullName,
            message.UserEmail
        );
    }

    private async Task RunCheckLoopAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scheduled check for overdue tasks...");

                var reminderCount = await _taskReminderService.CheckAndPublishOverdueTasksAsync(stoppingToken);

                _logger.LogInformation(
                    "Check complete. Published {Count} reminder(s). Next check in {Interval}",
                    reminderCount, CheckInterval);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Check loop cancelled due to shutdown");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled task check");
            }

            try
            {
                await Task.Delay(CheckInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Task Reminder Service is stopping...");
        _rabbitMQService.StopConsuming();
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Task Reminder Service stopped");
    }
}
