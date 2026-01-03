using Microsoft.EntityFrameworkCore;
using TasksManagerAPI.Data;
using TasksManagerAPI.Models;
using Microsoft.Extensions.Logging;

namespace TasksManagerAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskRepository> _logger;

        public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<UserTask>> GetAllAsync()
        {
            _logger.LogDebug("Fetching all tasks from database");
            var tasks = await _context.Tasks
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.DueDate)
                .AsNoTracking()
                .ToListAsync();
            _logger.LogDebug("Fetched {Count} tasks from database", tasks.Count);
            return tasks;
        }

        public async Task<UserTask?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Fetching task with ID: {TaskId} from database", id);
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (task == null)
            {
                _logger.LogDebug("Task with ID: {TaskId} not found in database", id);
            }
            return task;
        }

        public async Task<IEnumerable<UserTask>> GetOverdueTasksAsync()
        {
            _logger.LogDebug("Fetching overdue tasks from database");
            var tasks = await _context.Tasks
                .Where(t => t.DueDate < DateTime.UtcNow)
                .OrderBy(t => t.DueDate)
                .AsNoTracking()
                .ToListAsync();
            _logger.LogDebug("Fetched {Count} overdue tasks from database", tasks.Count);
            return tasks;
        }

        public async Task<UserTask> CreateAsync(UserTask task)
        {
            _logger.LogDebug("Creating new task in database with title: {Title}", task.Title);
            task.CreatedAt = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Task created in database with ID: {TaskId}", task.Id);
            return task;
        }

        public async Task<UserTask?> UpdateAsync(int id, UserTask task)
        {
            _logger.LogDebug("Updating task with ID: {TaskId} in database", id);
            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
            {
                _logger.LogDebug("Task with ID: {TaskId} not found for update in database", id);
                return null;
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Priority = task.Priority;
            existingTask.FullName = task.FullName;
            existingTask.Telephone = task.Telephone;
            existingTask.Email = task.Email;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogDebug("Task with ID: {TaskId} updated in database", id);
            return existingTask;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogDebug("Deleting task with ID: {TaskId} from database", id);
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                _logger.LogDebug("Task with ID: {TaskId} not found for deletion in database", id);
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            _logger.LogDebug("Task with ID: {TaskId} deleted from database", id);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            _logger.LogDebug("Checking if task with ID: {TaskId} exists in database", id);
            return await _context.Tasks.AnyAsync(t => t.Id == id);
        }
    }
}