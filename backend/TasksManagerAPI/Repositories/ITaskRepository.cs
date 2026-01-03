using TasksManagerAPI.Models;

namespace TasksManagerAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<UserTask>> GetAllAsync();
        Task<UserTask?> GetByIdAsync(int id);
        Task<IEnumerable<UserTask>> GetOverdueTasksAsync();
        Task<UserTask> CreateAsync(UserTask task);
        Task<UserTask?> UpdateAsync(int id, UserTask task);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}