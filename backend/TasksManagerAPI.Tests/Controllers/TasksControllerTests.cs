using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TasksManagerAPI.Controllers;
using TasksManagerAPI.DTOs;
using TasksManagerAPI.Services;
using TasksManagerAPI.Tests.Helpers;
using Xunit;

namespace TasksManagerAPI.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly Mock<ILogger<TasksController>> _loggerMock;
    private readonly TasksController _sut;

    public TasksControllerTests()
    {
        _serviceMock = new Mock<ITaskService>();
        _loggerMock = new Mock<ILogger<TasksController>>();
        _sut = new TasksController(_serviceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithTasks()
    {
        var tasks = new List<TaskResponseDto>
        {
            new() { Id = 1, Title = "Task 1", Description = "Desc", FullName = "John", Telephone = "123", Email = "a@b.com" },
            new() { Id = 2, Title = "Task 2", Description = "Desc", FullName = "Jane", Telephone = "456", Email = "c@d.com" }
        };
        _serviceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

        var result = await _sut.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedTasks.Count());
    }

    [Fact]
    public async Task GetById_WhenTaskExists_ReturnsOkWithTask()
    {
        // Arrange
        var task = new TaskResponseDto 
        { 
            Id = 1, 
            Title = "Test Task", 
            Description = "Desc", 
            FullName = "John", 
            Telephone = "123", 
            Email = "a@b.com" 
        };
        _serviceMock.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTask = Assert.IsType<TaskResponseDto>(okResult.Value);
        Assert.Equal("Test Task", returnedTask.Title);
    }

    [Fact]
    public async Task GetById_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetTaskByIdAsync(999)).ReturnsAsync((TaskResponseDto?)null);

        // Act
        var result = await _sut.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetOverdue_ReturnsOkWithOverdueTasks()
    {
        // Arrange
        var tasks = new List<TaskResponseDto>
        {
            new() { Id = 1, Title = "Overdue Task", Description = "Desc", FullName = "John", Telephone = "123", Email = "a@b.com" }
        };
        _serviceMock.Setup(s => s.GetOverdueTasksAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _sut.GetOverdue();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskResponseDto>>(okResult.Value);
        Assert.Single(returnedTasks);
    }

    [Fact]
    public async Task Create_WithValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var dto = TestDataBuilder.CreateTaskDto("New Task");
        var createdTask = new TaskResponseDto 
        { 
            Id = 1, 
            Title = "New Task", 
            Description = "New Description", 
            FullName = "Jane Doe", 
            Telephone = "098-765-4321", 
            Email = "jane.doe@example.com" 
        };
        _serviceMock.Setup(s => s.CreateTaskAsync(dto)).ReturnsAsync(createdTask);

        // Act
        var result = await _sut.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(TasksController.GetById), createdResult.ActionName);
        var returnedTask = Assert.IsType<TaskResponseDto>(createdResult.Value);
        Assert.Equal("New Task", returnedTask.Title);
    }

    [Fact]
    public async Task Update_WhenTaskExists_ReturnsOkWithUpdatedTask()
    {
        // Arrange
        var dto = TestDataBuilder.CreateUpdateTaskDto("Updated Task");
        var updatedTask = new TaskResponseDto 
        { 
            Id = 1, 
            Title = "Updated Task", 
            Description = "Updated Description", 
            FullName = "Jane Doe Updated", 
            Telephone = "111-222-3333", 
            Email = "jane.updated@example.com" 
        };
        _serviceMock.Setup(s => s.UpdateTaskAsync(1, dto)).ReturnsAsync(updatedTask);

        // Act
        var result = await _sut.Update(1, dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTask = Assert.IsType<TaskResponseDto>(okResult.Value);
        Assert.Equal("Updated Task", returnedTask.Title);
    }

    [Fact]
    public async Task Update_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        var dto = TestDataBuilder.CreateUpdateTaskDto();
        _serviceMock.Setup(s => s.UpdateTaskAsync(999, dto)).ReturnsAsync((TaskResponseDto?)null);

        // Act
        var result = await _sut.Update(999, dto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WhenTaskExists_ReturnsNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenTaskNotExists_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteTaskAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _sut.Delete(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}