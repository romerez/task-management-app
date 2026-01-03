using Microsoft.EntityFrameworkCore;
using TaskManagement.ReminderService.Data;
using TaskManagement.ReminderService.Models;

namespace TaskManagement.ReminderService.Services;

public interface ITaskReminderService
{
    Task<int> CheckAndPublishOverdueTasksAsync(CancellationToken cancellationToken = default);
}

public class TaskReminderService : ITaskReminderService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<TaskReminderService> _logger;

    public TaskReminderService(
        IServiceScopeFactory scopeFactory,
        IRabbitMQService rabbitMQService,
        ILogger<TaskReminderService> logger)
    {
        _scopeFactory = scopeFactory;
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    public async Task<int> CheckAndPublishOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking for overdue tasks...");

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReminderDbContext>();

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var remindersSent = 0;

        var overdueTasks = await dbContext.Tasks
            .Where(t => t.DueDate < now)
            .Where(t => t.LastReminderSentAt == null || t.LastReminderSentAt < todayStart)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Found {Count} overdue tasks needing reminders", overdueTasks.Count);

        foreach (var task in overdueTasks)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, stopping task processing");
                break;
            }

            try
            {
                var daysOverdue = (now - task.DueDate).Days;

                // Enhanced logging for each overdue task
                _logger.LogWarning(
                    "OVERDUE TASK DETECTED | TaskId: {TaskId} | Title: {Title} | DueDate: {DueDate:yyyy-MM-dd} | DaysOverdue: {DaysOverdue} | Priority: {Priority} | Owner: {FullName} | Email: {Email}",
                    task.Id,
                    task.Title,
                    task.DueDate,
                    daysOverdue,
                    task.Priority,
                    task.FullName,
                    task.Email);

                var message = new TaskReminderMessage
                {
                    TaskId = task.Id,
                    TaskTitle = task.Title,
                    DueDate = task.DueDate,
                    UserFullName = task.FullName,
                    UserEmail = task.Email,
                    CreatedAt = DateTime.UtcNow
                };

                _rabbitMQService.PublishReminder(message);
                task.LastReminderSentAt = DateTime.UtcNow;
                remindersSent++;

                _logger.LogDebug("Published reminder for task {TaskId}: {Title}", task.Id, task.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish reminder for task {TaskId}", task.Id);
            }
        }

        if (remindersSent > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated {Count} tasks with reminder timestamps", remindersSent);
        }

        return remindersSent;
    }
}