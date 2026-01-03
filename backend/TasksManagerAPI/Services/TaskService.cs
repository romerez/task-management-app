using TasksManagerAPI.DTOs;
using TasksManagerAPI.Models;
using TasksManagerAPI.Repositories;

namespace TasksManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
        {
            _logger.LogInformation("Getting all tasks");
            var tasks = await _repository.GetAllAsync();
            var result = tasks.Select(MapToResponseDto);
            _logger.LogInformation("Retrieved {Count} tasks", result.Count());
            return result;
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
        {
            _logger.LogInformation("Getting task by ID: {TaskId}", id);
            var task = await _repository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found", id);
                return null;
            }
            return MapToResponseDto(task);
        }

        public async Task<IEnumerable<TaskResponseDto>> GetOverdueTasksAsync()
        {
            _logger.LogInformation("Getting overdue tasks");
            var tasks = await _repository.GetOverdueTasksAsync();
            var result = tasks.Select(MapToResponseDto);
            _logger.LogInformation("Retrieved {Count} overdue tasks", result.Count());
            return result;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto)
        {
            _logger.LogInformation("Creating task with title: {Title}, Priority: {Priority}", dto.Title, dto.Priority);
            var task = new UserTask
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                FullName = dto.FullName.Trim(),
                Telephone = dto.Telephone.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant()
            };

            var createdTask = await _repository.CreateAsync(task);
            _logger.LogInformation("Task created with ID: {TaskId}", createdTask.Id);
            
            return MapToResponseDto(createdTask);
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskDto dto)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", id);
            var task = new UserTask
            {
                Title = dto.Title.Trim(),
                Description = dto.Description.Trim(),
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                FullName = dto.FullName.Trim(),
                Telephone = dto.Telephone.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant()
            };

            var updatedTask = await _repository.UpdateAsync(id, task);
            if (updatedTask == null)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found for update", id);
                return null;
            }

            _logger.LogInformation("Task updated with ID: {TaskId}", id);
            return MapToResponseDto(updatedTask);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", id);
            var result = await _repository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Task deleted with ID: {TaskId}", id);
            }
            else
            {
                _logger.LogWarning("Task with ID: {TaskId} not found for deletion", id);
            }
            return result;
        }

        private static TaskResponseDto MapToResponseDto(UserTask task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                FullName = task.FullName,
                Telephone = task.Telephone,
                Email = task.Email,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}