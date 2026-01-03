using TasksManagerAPI.DTOs;
using TasksManagerAPI.Models;

namespace TasksManagerAPI.Tests.Helpers;

public static class TestDataBuilder
{
    public static UserTask CreateUserTask(
        int id = 1,
        string title = "Test Task",
        string description = "Test Description",
        Priority priority = Priority.Medium,
        DateTime? dueDate = null)
    {
        return new UserTask
        {
            Id = id,
            Title = title,
            Description = description,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(7),
            Priority = priority,
            FullName = "John Doe",
            Telephone = "123-456-7890",
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static CreateTaskDto CreateTaskDto(
        string title = "New Task",
        string description = "New Description",
        Priority priority = Priority.Medium,
        DateTime? dueDate = null)
    {
        return new CreateTaskDto
        {
            Title = title,
            Description = description,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(7),
            Priority = priority,
            FullName = "Jane Doe",
            Telephone = "098-765-4321",
            Email = "jane.doe@example.com"
        };
    }

    public static UpdateTaskDto CreateUpdateTaskDto(
        string title = "Updated Task",
        string description = "Updated Description",
        Priority priority = Priority.High,
        DateTime? dueDate = null)
    {
        return new UpdateTaskDto
        {
            Title = title,
            Description = description,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(14),
            Priority = priority,
            FullName = "Jane Doe Updated",
            Telephone = "111-222-3333",
            Email = "jane.updated@example.com"
        };
    }
}