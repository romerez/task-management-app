using Microsoft.Extensions.Logging;
using Moq;
using TasksManagerAPI.DTOs;
using TasksManagerAPI.Models;
using TasksManagerAPI.Repositories;
using TasksManagerAPI.Services;
using TasksManagerAPI.Tests.Helpers;
using Xunit;

namespace TasksManagerAPI.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly Mock<ILogger<TaskService>> _loggerMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _loggerMock = new Mock<ILogger<TaskService>>();
        _sut = new TaskService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllTasksAsync_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<UserTask>
        {
            TestDataBuilder.CreateUserTask(1, "Task 1"),
            TestDataBuilder.CreateUserTask(2, "Task 2")
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetAllTasksAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WhenTaskExists_ReturnsTask()
    {
        // Arrange
        var task = TestDataBuilder.CreateUserTask(1, "Test Task");
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _sut.GetTaskByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WhenTaskNotExists_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((UserTask?)null);

        // Act
        var result = await _sut.GetTaskByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetOverdueTasksAsync_ReturnsOverdueTasks()
    {
        // Arrange
        var overdueTasks = new List<UserTask>
        {
            TestDataBuilder.CreateUserTask(1, "Overdue Task", dueDate: DateTime.UtcNow.AddDays(-1))
        };
        _repositoryMock.Setup(r => r.GetOverdueTasksAsync()).ReturnsAsync(overdueTasks);

        // Act
        var result = await _sut.GetOverdueTasksAsync();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateTaskAsync_CreatesAndReturnsTask()
    {
        // Arrange
        var dto = TestDataBuilder.CreateTaskDto("New Task");
        var createdTask = TestDataBuilder.CreateUserTask(1, "New Task");
        
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserTask>())).ReturnsAsync(createdTask);

        // Act
        var result = await _sut.CreateTaskAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Task", result.Title);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<UserTask>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_TrimsInputFields()
    {
        // Arrange
        var dto = TestDataBuilder.CreateTaskDto("  Task with spaces  ");
        var createdTask = TestDataBuilder.CreateUserTask(1, "Task with spaces");
        
        _repositoryMock.Setup(r => r.CreateAsync(It.Is<UserTask>(t => t.Title == "Task with spaces")))
            .ReturnsAsync(createdTask);

        // Act
        var result = await _sut.CreateTaskAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.CreateAsync(It.Is<UserTask>(t => t.Title == "Task with spaces")), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskExists_ReturnsUpdatedTask()
    {
        // Arrange
        var dto = TestDataBuilder.CreateUpdateTaskDto("Updated Task");
        var updatedTask = TestDataBuilder.CreateUserTask(1, "Updated Task");
        
        _repositoryMock.Setup(r => r.UpdateAsync(1, It.IsAny<UserTask>())).ReturnsAsync(updatedTask);

        // Act
        var result = await _sut.UpdateTaskAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Task", result.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenTaskNotExists_ReturnsNull()
    {
        // Arrange
        var dto = TestDataBuilder.CreateUpdateTaskDto();
        _repositoryMock.Setup(r => r.UpdateAsync(999, It.IsAny<UserTask>())).ReturnsAsync((UserTask?)null);

        // Act
        var result = await _sut.UpdateTaskAsync(999, dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WhenTaskExists_ReturnsTrue()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteTaskAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_WhenTaskNotExists_ReturnsFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _sut.DeleteTaskAsync(999);

        // Assert
        Assert.False(result);
    }
}