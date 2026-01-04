using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.ReminderService.Data;
using TaskManagement.ReminderService.Models;
using TaskManagement.ReminderService.Services;
using Xunit;

namespace TaskManagement.ReminderService.Tests.Services;

public class TaskReminderServiceTests : IDisposable
{
    private readonly ReminderDbContext _dbContext;
    private readonly Mock<IRabbitMQService> _mockRabbitMQService;
    private readonly Mock<ILogger<TaskReminderService>> _mockLogger;
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly TaskReminderService _sut;

    public TaskReminderServiceTests()
    {
        var options = new DbContextOptionsBuilder<ReminderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ReminderDbContext(options);
        _mockRabbitMQService = new Mock<IRabbitMQService>();
        _mockLogger = new Mock<ILogger<TaskReminderService>>();
        _mockScopeFactory = CreateMockScopeFactory(_dbContext);

        _sut = new TaskReminderService(
            _mockScopeFactory.Object,
            _mockRabbitMQService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_WithNoOverdueTasks_ReturnsZero()
    {
        // Arrange
        var futureTask = new UserTask
        {
            Id = 1,
            Title = "Future Task",
            Description = "A task due in the future",
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = 1,
            FullName = "John Doe",
            Telephone = "123-456-7890",
            Email = "john@example.com"
        };
        _dbContext.Tasks.Add(futureTask);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.CheckAndPublishOverdueTasksAsync();

        // Assert
        Assert.Equal(0, result);
        _mockRabbitMQService.Verify(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()), Times.Never);
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_WithOverdueTasks_PublishesReminders()
    {
        // Arrange
        var overdueTask = new UserTask
        {
            Id = 1,
            Title = "Overdue Task",
            Description = "A task that is overdue",
            DueDate = DateTime.UtcNow.AddDays(-3),
            Priority = 2,
            FullName = "Jane Doe",
            Telephone = "098-765-4321",
            Email = "jane@example.com",
            LastReminderSentAt = null
        };
        _dbContext.Tasks.Add(overdueTask);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.CheckAndPublishOverdueTasksAsync();

        // Assert
        Assert.Equal(1, result);
        _mockRabbitMQService.Verify(
            x => x.PublishReminder(It.Is<TaskReminderMessage>(m =>
                m.TaskId == overdueTask.Id &&
                m.TaskTitle == overdueTask.Title &&
                m.UserEmail == overdueTask.Email)),
            Times.Once);
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_SkipsTasksWithRecentReminder()
    {
        // Arrange
        var overdueTaskWithRecentReminder = new UserTask
        {
            Id = 1,
            Title = "Already Reminded Task",
            Description = "Task with reminder sent today",
            DueDate = DateTime.UtcNow.AddDays(-1),
            Priority = 1,
            FullName = "Bob Smith",
            Telephone = "111-222-3333",
            Email = "bob@example.com",
            LastReminderSentAt = DateTime.UtcNow.AddHours(-2) // Sent today
        };
        _dbContext.Tasks.Add(overdueTaskWithRecentReminder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.CheckAndPublishOverdueTasksAsync();

        // Assert
        Assert.Equal(0, result);
        _mockRabbitMQService.Verify(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()), Times.Never);
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_ProcessesMultipleOverdueTasks()
    {
        // Arrange
        var tasks = new List<UserTask>
        {
            new()
            {
                Id = 1,
                Title = "Overdue Task 1",
                Description = "First overdue task",
                DueDate = DateTime.UtcNow.AddDays(-5),
                Priority = 3,
                FullName = "User One",
                Telephone = "111-111-1111",
                Email = "user1@example.com"
            },
            new()
            {
                Id = 2,
                Title = "Overdue Task 2",
                Description = "Second overdue task",
                DueDate = DateTime.UtcNow.AddDays(-2),
                Priority = 1,
                FullName = "User Two",
                Telephone = "222-222-2222",
                Email = "user2@example.com"
            }
        };
        _dbContext.Tasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.CheckAndPublishOverdueTasksAsync();

        // Assert
        Assert.Equal(2, result);
        _mockRabbitMQService.Verify(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_UpdatesLastReminderSentAt()
    {
        // Arrange
        var overdueTask = new UserTask
        {
            Id = 1,
            Title = "Overdue Task",
            Description = "Task to update",
            DueDate = DateTime.UtcNow.AddDays(-1),
            Priority = 1,
            FullName = "Test User",
            Telephone = "333-333-3333",
            Email = "test@example.com",
            LastReminderSentAt = null
        };
        _dbContext.Tasks.Add(overdueTask);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.CheckAndPublishOverdueTasksAsync();

        // Assert
        var updatedTask = await _dbContext.Tasks.FindAsync(1);
        Assert.NotNull(updatedTask?.LastReminderSentAt);
        Assert.True(updatedTask.LastReminderSentAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_StopsOnCancellation()
    {
        var tasks = Enumerable.Range(1, 5).Select(i => new UserTask
        {
            Id = i,
            Title = $"Overdue Task {i}",
            Description = $"Description {i}",
            DueDate = DateTime.UtcNow.AddDays(-i),
            Priority = 1,
            FullName = $"User {i}",
            Telephone = "444-444-4444",
            Email = $"user{i}@example.com"
        }).ToList();

        _dbContext.Tasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync();

        using var cts = new CancellationTokenSource();
        var publishCount = 0;

        _mockRabbitMQService
            .Setup(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()))
            .Callback(() =>
            {
                publishCount++;
                if (publishCount >= 2)
                {
                    cts.Cancel();
                }
            });

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _sut.CheckAndPublishOverdueTasksAsync(cts.Token));

        _mockRabbitMQService.Verify(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()), Times.Exactly(2));
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_ThrowsWhenCancelledBeforeQuery()
    {
        // Arrange
        var tasks = Enumerable.Range(1, 3).Select(i => new UserTask
        {
            Id = i,
            Title = $"Overdue Task {i}",
            Description = $"Description {i}",
            DueDate = DateTime.UtcNow.AddDays(-i),
            Priority = 1,
            FullName = $"User {i}",
            Telephone = "444-444-4444",
            Email = $"user{i}@example.com"
        }).ToList();

        _dbContext.Tasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync();

        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel before calling the method

        // Act & Assert - Should throw OperationCanceledException when token is already cancelled
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.CheckAndPublishOverdueTasksAsync(cts.Token));
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_LogsWarningForEachOverdueTask()
    {
        // Arrange
        var overdueTask = new UserTask
        {
            Id = 1,
            Title = "Important Overdue Task",
            Description = "Critical task",
            DueDate = DateTime.UtcNow.AddDays(-7),
            Priority = 3,
            FullName = "Manager",
            Telephone = "555-555-5555",
            Email = "manager@example.com"
        };
        _dbContext.Tasks.Add(overdueTask);
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.CheckAndPublishOverdueTasksAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("OVERDUE TASK DETECTED")),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CheckAndPublishOverdueTasksAsync_ContinuesProcessingOnPublishError()
    {
        var tasks = new List<UserTask>
        {
            new()
            {
                Id = 1,
                Title = "Task That Fails",
                Description = "This will fail to publish",
                DueDate = DateTime.UtcNow.AddDays(-1),
                Priority = 1,
                FullName = "Fail User",
                Telephone = "666-666-6666",
                Email = "fail@example.com"
            },
            new()
            {
                Id = 2,
                Title = "Task That Succeeds",
                Description = "This will publish successfully",
                DueDate = DateTime.UtcNow.AddDays(-2),
                Priority = 2,
                FullName = "Success User",
                Telephone = "777-777-7777",
                Email = "success@example.com"
            }
        };
        _dbContext.Tasks.AddRange(tasks);
        await _dbContext.SaveChangesAsync();

        _mockRabbitMQService
            .SetupSequence(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()))
            .Throws(new Exception("Publish failed"))
            .Pass();

        var result = await _sut.CheckAndPublishOverdueTasksAsync();

        Assert.Equal(1, result); 
        _mockRabbitMQService.Verify(x => x.PublishReminder(It.IsAny<TaskReminderMessage>()), Times.Exactly(2));
    }

    private static Mock<IServiceScopeFactory> CreateMockScopeFactory(ReminderDbContext dbContext)
    {
        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(x => x.GetService(typeof(ReminderDbContext)))
            .Returns(dbContext);

        var mockScope = new Mock<IServiceScope>();
        mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);

        return mockScopeFactory;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}