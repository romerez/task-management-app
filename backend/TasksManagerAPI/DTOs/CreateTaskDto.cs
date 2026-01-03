using System.ComponentModel.DataAnnotations;
using TasksManagerAPI.Models;

namespace TasksManagerAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
        public required string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public required string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Due date is required")]
        public required DateTime DueDate { get; set; }  

        [Required(ErrorMessage = "Priority is required")]
        public required Priority Priority { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public required string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telephone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Telephone cannot exceed 20 characters")]
        public required string Telephone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public required string Email { get; set; } = string.Empty;
    }
}