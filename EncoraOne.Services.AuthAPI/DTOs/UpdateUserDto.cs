using System;
using System.ComponentModel.DataAnnotations;

namespace EncoraOne.Grievance.API.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; } // Required to identify the user being updated

        [MaxLength(100)]
        public string? FullName { get; set; }

        // Email is usually kept the same, but allowing it to be present for consistency
        [EmailAddress]
        public string? Email { get; set; }

        // Password is optional: the user might only update the name or role. 
        // The service layer must handle hashing this if it's provided.
        public string? Password { get; set; }

        // Role must be provided (1 for Admin, 2 for Manager, 3 for Employee)
        public int? Role { get; set; }

        // Optional fields based on role
        public int? DepartmentId { get; set; }

        [MaxLength(50)]
        public string? JobTitle { get; set; }

        // You might want to include IsActive status here for enabling/disabling users
        public bool? IsActive { get; set; }
    }
}