using Microsoft.AspNetCore.Mvc;
using TasksManagerAPI.DTOs;
using TasksManagerAPI.Services;

namespace TasksManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving all tasks");
            var tasks = await _taskService.GetAllTasksAsync();
            _logger.LogInformation("Retrieved {Count} tasks", tasks.Count());
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> GetById(int id)
        {
            _logger.LogInformation("Retrieving task with ID: {TaskId}", id);
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found", id);
                return NotFound(new { message = $"Task with ID {id} not found" });
            }
            _logger.LogInformation("Successfully retrieved task with ID: {TaskId}", id);
            return Ok(task);
        }

        [HttpGet("overdue")]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetOverdue()
        {
            _logger.LogInformation("Retrieving overdue tasks");
            var tasks = await _taskService.GetOverdueTasksAsync();
            _logger.LogInformation("Retrieved {Count} overdue tasks", tasks.Count());
            return Ok(tasks);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskResponseDto>> Create([FromBody] CreateTaskDto dto)
        {
            _logger.LogInformation("Creating new task with title: {Title}", dto.Title);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task creation");
                return BadRequest(ModelState);
            }

            var createdTask = await _taskService.CreateTaskAsync(dto);
            _logger.LogInformation("Task created successfully with ID: {TaskId}", createdTask.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskResponseDto>> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            _logger.LogInformation("Updating task with ID: {TaskId}", id);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task update, ID: {TaskId}", id);
                return BadRequest(ModelState);
            }                           

            var updatedTask = await _taskService.UpdateTaskAsync(id, dto);
            if (updatedTask == null)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found for update", id);
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            _logger.LogInformation("Task with ID: {TaskId} updated successfully", id);
            return Ok(updatedTask);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting task with ID: {TaskId}", id);
                var result = await _taskService.DeleteTaskAsync(id);
            if (!result)
            {
                _logger.LogWarning("Task with ID: {TaskId} not found for deletion", id);
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            _logger.LogInformation("Task with ID: {TaskId} deleted successfully", id);
            return NoContent();
        }
    }
}