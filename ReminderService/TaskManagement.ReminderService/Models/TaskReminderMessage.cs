namespace TaskManagement.ReminderService.Models;

public class TaskReminderMessage
{
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int DaysOverdue => (DateTime.UtcNow - DueDate).Days;
}
