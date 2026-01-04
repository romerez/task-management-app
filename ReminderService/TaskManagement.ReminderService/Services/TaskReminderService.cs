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

        var overdueTasks = await GetOverdueTasksAsync(dbContext, cancellationToken);

        _logger.LogInformation("Found {Count} overdue tasks needing reminders", overdueTasks.Count);

        var remindersSent = await ProcessOverdueTasksAsync(overdueTasks, cancellationToken);

        if (remindersSent > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated {Count} tasks with reminder timestamps", remindersSent);
        }

        return remindersSent;
    }

    private static async Task<List<UserTask>> GetOverdueTasksAsync(
        ReminderDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;

        return await dbContext.Tasks
            .Where(t => t.DueDate < now)
            .Where(t => t.LastReminderSentAt == null || t.LastReminderSentAt < todayStart)
            .OrderBy(t => t.DueDate)
            .ToListAsync(cancellationToken);
    }

    private Task<int> ProcessOverdueTasksAsync(
        List<UserTask> tasks,
        CancellationToken cancellationToken)
    {
        var remindersSent = 0;

        foreach (var task in tasks)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Cancellation requested, stopping task processing");
                break;
            }

            if (TryPublishReminder(task))
            {
                remindersSent++;
            }
        }

        return Task.FromResult(remindersSent);
    }

    private bool TryPublishReminder(UserTask task)
    {
        try
        {
            LogOverdueTask(task);

            var message = CreateReminderMessage(task);
            _rabbitMQService.PublishReminder(message);

            task.LastReminderSentAt = DateTime.UtcNow;

            _logger.LogDebug("Published reminder for task {TaskId}: {Title}", task.Id, task.Title);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish reminder for task {TaskId}", task.Id);
            return false;
        }
    }

    private void LogOverdueTask(UserTask task)
    {
        var daysOverdue = (DateTime.UtcNow - task.DueDate).Days;

        _logger.LogWarning(
            "OVERDUE TASK DETECTED | TaskId: {TaskId} | Title: {Title} | DueDate: {DueDate:yyyy-MM-dd} | DaysOverdue: {DaysOverdue} | Priority: {Priority} | Owner: {FullName} | Email: {Email}",
            task.Id,
            task.Title,
            task.DueDate,
            daysOverdue,
            task.Priority,
            task.FullName,
            task.Email);
    }

    private static TaskReminderMessage CreateReminderMessage(UserTask task) => new()
    {
        TaskId = task.Id,
        TaskTitle = task.Title,
        DueDate = task.DueDate,
        UserFullName = task.FullName,
        UserEmail = task.Email,
        CreatedAt = DateTime.UtcNow
    };
}